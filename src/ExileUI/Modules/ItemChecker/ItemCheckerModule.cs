using ExileUI.Core;
using ExileUI.Infrastructure;

namespace ExileUI.Modules;

/// <summary>
/// Reads item text via clipboard and shows an info tooltip. Replacement for AHK item-checker module.
/// </summary>
public class ItemCheckerModule : ModuleBase
{
    public ItemCheckerModule(AppState state, ConfigManager config, WindowManager windows, HotkeyManager hotkeys)
        : base(state, config, windows, hotkeys) { }

    public override string Name => "ItemChecker";

    public override void Initialize()
    {
        // TODO: implement
    }

    public override void Shutdown()
    {
        Windows.DestroyAll();
    }
}
