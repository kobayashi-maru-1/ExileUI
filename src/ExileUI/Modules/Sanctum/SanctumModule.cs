using ExileUI.Core;
using ExileUI.Infrastructure;

namespace ExileUI.Modules;

/// <summary>
/// Sanctum/Sekhema floor planner and relic manager. Replacement for AHK sanctum module.
/// </summary>
public class SanctumModule : ModuleBase
{
    public SanctumModule(AppState state, ConfigManager config, WindowManager windows, HotkeyManager hotkeys)
        : base(state, config, windows, hotkeys) { }

    public override string Name => "Sanctum";

    public override void Initialize()
    {
        // TODO: implement
    }

    public override void Shutdown()
    {
        Windows.DestroyAll();
    }
}
