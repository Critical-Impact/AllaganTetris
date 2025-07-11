﻿namespace AllaganTetris.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DalaMock.Host.Factories;
using DalaMock.Host.Mediator;
using AllaganTetris.Mediator;
using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class WindowService : DisposableMediatorSubscriberBase, IHostedService
{
    private readonly WindowSystemFactory windowSystemFactory;
    private readonly IUiBuilder uiBuilder;
    private readonly IWindowSystem windowSystem;

    public WindowService(IEnumerable<Window> windows, ILogger<DisposableMediatorSubscriberBase> logger,
        MediatorService mediatorService, WindowSystemFactory windowSystemFactory, IUiBuilder uiBuilder) : base(logger,
        mediatorService)
    {
        this.windowSystemFactory = windowSystemFactory;
        this.uiBuilder = uiBuilder;
        this.windowSystem = this.windowSystemFactory.Create("AllaganTetris");
        foreach (var window in windows)
        {
            this.windowSystem.AddWindow(window);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.MediatorService.Subscribe<OpenWindowMessage>(this, this.OpenWindow);
        this.MediatorService.Subscribe<ToggleWindowMessage>(this, this.ToggleWindow);
        this.MediatorService.Subscribe<CloseWindowMessage>(this, this.CloseWindow);

        this.uiBuilder.Draw += this.Draw;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.uiBuilder.Draw -= this.Draw;
        return Task.CompletedTask;
    }

    private void OpenWindow(OpenWindowMessage obj)
    {
        var window = this.windowSystem.Windows.FirstOrDefault(c => c.GetType() == obj.WindowType);
        if (window != null)
        {
            window.IsOpen = true;
        }
    }

    private void CloseWindow(CloseWindowMessage obj)
    {
        var window = this.windowSystem.Windows.FirstOrDefault(c => c.GetType() == obj.WindowType);
        if (window != null)
        {
            window.IsOpen = false;
        }
    }

    private void ToggleWindow(ToggleWindowMessage obj)
    {
        var window = this.windowSystem.Windows.FirstOrDefault(c => c.GetType() == obj.WindowType);
        if (window != null)
        {
            window.IsOpen = !window.IsOpen;
        }
    }

    private void Draw()
    {
        this.windowSystem.Draw();
    }
}