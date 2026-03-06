using ExileUI.Core;
using ExileUI.Infrastructure;

namespace ExileUI.Modules;

/// <summary>
/// Act guide overlay with gem setup and skill tree. Replacement for AHK leveling tracker module.
/// </summary>
public class LevelTrackerModule : ModuleBase
{
    public LevelTrackerModule(AppState state, ConfigManager config, WindowManager windows, HotkeyManager hotkeys)
        : base(state, config, windows, hotkeys) { }

    public override string Name => "LevelTracker";

    public override void Initialize()
    {
        // TODO: implement
    }

    public override void Shutdown()
    {
        Windows.DestroyAll();
    }
}
