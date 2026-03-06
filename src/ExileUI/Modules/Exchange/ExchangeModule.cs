using ExileUI.Core;
using ExileUI.Infrastructure;

namespace ExileUI.Modules;

/// <summary>
/// Vaal Street currency exchange tracker. Replacement for AHK exchange module.
/// </summary>
public class ExchangeModule : ModuleBase
{
    public ExchangeModule(AppState state, ConfigManager config, WindowManager windows, HotkeyManager hotkeys)
        : base(state, config, windows, hotkeys) { }

    public override string Name => "Exchange";

    public override void Initialize()
    {
        // TODO: implement
    }

    public override void Shutdown()
    {
        Windows.DestroyAll();
    }
}
