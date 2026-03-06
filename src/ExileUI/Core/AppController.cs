using ExileUI.Infrastructure;
using ExileUI.Modules;

namespace ExileUI.Core;

/// <summary>
/// Orchestrates startup, module initialization, and shutdown.
/// Equivalent to the main script body in AHK.
/// </summary>
public class AppController
{
    private readonly AppState _state;
    private readonly GameClient _gameClient;
    private readonly HotkeyManager _hotkeys;
    private readonly ClientLogMonitor _logMonitor;
    private readonly WindowManager _windowManager;
    private readonly TrayIconManager _tray;
    private readonly ConfigManager _config;
    private readonly IEnumerable<IModule> _modules;

    public AppController(
        AppState state,
        GameClient gameClient,
        HotkeyManager hotkeys,
        ClientLogMonitor logMonitor,
        WindowManager windowManager,
        TrayIconManager tray,
        ConfigManager config,
        IEnumerable<IModule> modules)
    {
        _state = state;
        _gameClient = gameClient;
        _hotkeys = hotkeys;
        _logMonitor = logMonitor;
        _windowManager = windowManager;
        _tray = tray;
        _config = config;
        _modules = modules;
    }

    public void Start()
    {
        _tray.Initialize();
        _config.Load();

        _gameClient.ClientFound += OnClientFound;
        _gameClient.ClientLost += OnClientLost;
        _gameClient.StartPolling();
    }

    private void OnClientFound()
    {
        _logMonitor.Start();

        foreach (var module in _modules)
        {
            if (module.IsEnabled)
                module.Initialize();
        }

        _hotkeys.Start();
    }

    private void OnClientLost()
    {
        _hotkeys.Stop();

        foreach (var module in _modules)
            module.Shutdown();

        _logMonitor.Stop();
    }

    public void Stop()
    {
        _gameClient.StopPolling();
        OnClientLost();
        _tray.Dispose();
    }
}
