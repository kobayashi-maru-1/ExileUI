using ExileUI.Core;
using ExileUI.Infrastructure;

namespace ExileUI.Modules;

/// <summary>
/// Custom context-sensitive cheat sheet overlays. Replacement for AHK cheat-sheets module.
/// </summary>
public class CheatSheetsModule : ModuleBase
{
    public CheatSheetsModule(AppState state, ConfigManager config, WindowManager windows, HotkeyManager hotkeys)
        : base(state, config, windows, hotkeys) { }

    public override string Name => "CheatSheets";

    public override void Initialize()
    {
        // TODO: implement
    }

    public override void Shutdown()
    {
        Windows.DestroyAll();
    }
}
