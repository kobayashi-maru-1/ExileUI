namespace ExileUI.Modules;

/// <summary>
/// Contract for all feature modules.
/// Each module manages its own overlay windows, hotkeys, and state.
/// </summary>
public interface IModule
{
    /// <summary>Module name, used for config lookups and logging.</summary>
    string Name { get; }

    /// <summary>Whether this module is enabled in settings.</summary>
    bool IsEnabled { get; }

    /// <summary>Called once when the game client is first detected.</summary>
    void Initialize();

    /// <summary>Called when the game client closes or the app exits.</summary>
    void Shutdown();
}
