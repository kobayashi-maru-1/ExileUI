using ExileUI.Core;
using ExileUI.Infrastructure;

namespace ExileUI.Modules;

/// <summary>
/// In-game loot filter management. Replacement for AHK lootfilter module.
/// </summary>
public class LootFilterModule : ModuleBase
{
    public LootFilterModule(AppState state, ConfigManager config, WindowManager windows, HotkeyManager hotkeys)
        : base(state, config, windows, hotkeys) { }

    public override string Name => "LootFilter";

    public override void Initialize()
    {
        // TODO: implement
    }

    public override void Shutdown()
    {
        Windows.DestroyAll();
    }
}
