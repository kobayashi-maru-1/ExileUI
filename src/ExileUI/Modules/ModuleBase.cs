using ExileUI.Core;
using ExileUI.Infrastructure;

namespace ExileUI.Modules;

/// <summary>
/// Convenience base for modules — provides access to shared services.
/// </summary>
public abstract class ModuleBase : IModule
{
    protected readonly AppState State;
    protected readonly ConfigManager Config;
    protected readonly WindowManager Windows;
    protected readonly HotkeyManager Hotkeys;

    protected ModuleBase(AppState state, ConfigManager config, WindowManager windows, HotkeyManager hotkeys)
    {
        State = state;
        Config = config;
        Windows = windows;
        Hotkeys = hotkeys;
    }

    public abstract string Name { get; }

    public virtual bool IsEnabled => Config.Get<bool>(Name, "enabled", true);

    public virtual void Initialize() { }

    public virtual void Shutdown() { }
}
