using System.Windows;
using Application = System.Windows.Application;
using Microsoft.Extensions.DependencyInjection;
using ExileUI.Core;
using ExileUI.Infrastructure;
using ExileUI.Modules;

namespace ExileUI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        // Start the application
        Services.GetRequiredService<AppController>().Start();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Core
        services.AddSingleton<AppState>();
        services.AddSingleton<AppController>();
        services.AddSingleton<GameClient>();

        // Infrastructure
        services.AddSingleton<HotkeyManager>();
        services.AddSingleton<ClientLogMonitor>();
        services.AddSingleton<ScreenCaptureService>();
        services.AddSingleton<ConfigManager>();
        services.AddSingleton<WindowManager>();
        services.AddSingleton<TrayIconManager>();

        // Modules
        services.AddSingleton<IModule, CloneFramesModule>();
        services.AddSingleton<IModule, ItemCheckerModule>();
        services.AddSingleton<IModule, MapInfoModule>();
        services.AddSingleton<IModule, MapTrackerModule>();
        services.AddSingleton<IModule, LevelTrackerModule>();
        services.AddSingleton<IModule, ActDecoderModule>();
        services.AddSingleton<IModule, StashNinjaModule>();
        services.AddSingleton<IModule, SanctumModule>();
        services.AddSingleton<IModule, ChatMacrosModule>();
        services.AddSingleton<IModule, ExchangeModule>();
        services.AddSingleton<IModule, AnoIntsModule>();
        services.AddSingleton<IModule, BetrayalModule>();
        services.AddSingleton<IModule, CheatSheetsModule>();
        services.AddSingleton<IModule, SearchStringsModule>();
        services.AddSingleton<IModule, RecombinationModule>();
        services.AddSingleton<IModule, StatlasModule>();
        services.AddSingleton<IModule, OcrModule>();
        services.AddSingleton<IModule, QolToolsModule>();
        services.AddSingleton<IModule, LootFilterModule>();
        services.AddSingleton<IModule, SeedExplorerModule>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Services.GetService<AppController>()?.Stop();
        base.OnExit(e);
    }
}
