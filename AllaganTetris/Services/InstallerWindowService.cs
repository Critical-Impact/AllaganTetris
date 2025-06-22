namespace AllaganTetris.Services;

using System.Threading;
using System.Threading.Tasks;
using DalaMock.Host.Mediator;
using AllaganTetris.Mediator;
using Dalamud.Plugin;
using Microsoft.Extensions.Hosting;

public class InstallerWindowService(
    IDalamudPluginInterface pluginInterface,
    MediatorService mediatorService) : IHostedService
{
    private readonly MediatorService mediatorService = mediatorService;

    public IDalamudPluginInterface PluginInterface { get; } = pluginInterface;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.PluginInterface.UiBuilder.OpenMainUi += this.ToggleMainUi;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.PluginInterface.UiBuilder.OpenMainUi -= this.ToggleMainUi;
        return Task.CompletedTask;
    }

    private void ToggleMainUi()
    {
        this.mediatorService.Publish(new ToggleTetrisWindowMessage());
    }
}