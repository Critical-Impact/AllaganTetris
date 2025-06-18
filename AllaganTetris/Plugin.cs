using AllaganTetris.Addon;
using AllaganTetris.Tetris;
using KamiToolKit;

namespace AllaganTetris;

using System.Reflection;
using Autofac;
using DalaMock.Host.Hosting;
using AllaganTetris.Services;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;

public class Plugin : HostedPlugin
{
    public Plugin(
        IDalamudPluginInterface pluginInterface,
        IPluginLog pluginLog,
        IFramework framework,
        ICommandManager commandManager,
        IDataManager dataManager,
        ITextureProvider textureProvider,
        IChatGui chatGui,
        IKeyState keyState,
        IDtrBar dtrBar)
        : base(pluginInterface, pluginLog, framework, commandManager, dataManager, textureProvider, chatGui, dtrBar, keyState)
    {
        this.CreateHost();
        this.Start();
    }

    /// <summary>
    /// Configures the optional services to register automatically for use in your plugin.
    /// </summary>
    /// <returns>A HostedPluginOptions configured with the options you require.</returns>
    public override HostedPluginOptions ConfigureOptions()
    {
        return new HostedPluginOptions()
        {
            UseMediatorService = true,
        };
    }

    public override void ConfigureContainer(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<WindowService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        containerBuilder.RegisterType<ConfigurationService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        containerBuilder.RegisterType<InstallerWindowService>().AsSelf().AsImplementedInterfaces().SingleInstance();

        containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Window"))
            .As<Window>()
            .AsSelf()
            .AsImplementedInterfaces();

        containerBuilder.Register(s =>
        {
            var configurationLoaderService = s.Resolve<ConfigurationService>();
            return configurationLoaderService.GetConfiguration();
        }).SingleInstance();

        //Tetris specific things
        containerBuilder.RegisterType<AddonService>().SingleInstance().AsImplementedInterfaces().AsSelf();
        containerBuilder.RegisterType<NativeController>().SingleInstance().AsImplementedInterfaces().AsSelf();
        containerBuilder.RegisterType<Game>().AsImplementedInterfaces().AsSelf();
        containerBuilder.RegisterType<TetrisAddon>().SingleInstance().AsSelf();
    }


    public override void ConfigureServices(IServiceCollection serviceCollection)
    {
    }
}