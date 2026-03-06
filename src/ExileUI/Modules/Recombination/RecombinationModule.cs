using ExileUI.Core;
using ExileUI.Infrastructure;

namespace ExileUI.Modules;

/// <summary>
/// Recombination simulator. Replacement for AHK recombination module.
/// </summary>
public class RecombinationModule : ModuleBase
{
    public RecombinationModule(AppState state, ConfigManager config, WindowManager windows, HotkeyManager hotkeys)
        : base(state, config, windows, hotkeys) { }

    public override string Name => "Recombination";

    public override void Initialize()
    {
        // TODO: implement
    }

    public override void Shutdown()
    {
        Windows.DestroyAll();
    }
}
