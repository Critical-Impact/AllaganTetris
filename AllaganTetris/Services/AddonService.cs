using System;
using System.Threading;
using AllaganTetris.Addon;
using AllaganTetris.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using KamiToolKit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace AllaganTetris.Services;

public class AddonService : DisposableMediatorSubscriberBase, IHostedService
{
    private readonly NativeController nativeController;
    private readonly TetrisAddon tetrisAddon;
    private readonly IFramework framework;
    private readonly ICommandManager commandManager;


    public AddonService(ILogger<AddonService> logger, MediatorService mediatorService, NativeController nativeController, TetrisAddon tetrisAddon, IFramework framework, ICommandManager commandManager) : base(logger, mediatorService)
    {
        this.nativeController = nativeController;
        this.tetrisAddon = tetrisAddon;
        this.framework = framework;
        this.commandManager = commandManager;
        this.MediatorService.Subscribe<ToggleTetrisWindowMessage>(this,Action );
    }

    private void Action(ToggleTetrisWindowMessage obj)
    {
        tetrisAddon.Toggle();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        commandManager.AddHandler("/tetris", new CommandInfo(OpenTetris)
        {
            HelpMessage = "Opens the tetris window."
        });
        return Task.CompletedTask;
    }

    private void OpenTetris(string command, string arguments)
    {
        tetrisAddon.Open();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            framework.RunOnFrameworkThread(() =>
            {
                tetrisAddon.Dispose();
                nativeController.Dispose();
            });
        }

        base.Dispose(disposing);
    }
}
