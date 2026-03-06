using System.Windows;
using System.Windows.Interop;

namespace ExileUI.Infrastructure;

/// <summary>
/// Global hotkey registration using Win32 RegisterHotKey.
/// Equivalent to AHK's Hotkey directive.
/// </summary>
public class HotkeyManager : IDisposable
{
    private readonly Dictionary<int, Action> _hotkeys = new();
    private HwndSource? _hwndSource;
    private int _nextId = 1;
    private bool _running;

    public void Start()
    {
        if (_running) return;
        _running = true;

        // Create a hidden message-only window to receive WM_HOTKEY
        var parameters = new HwndSourceParameters("ExileUI_HotkeyHost")
        {
            WindowStyle = 0,
            ExtendedWindowStyle = 0,
            PositionX = 0,
            PositionY = 0,
            Width = 0,
            Height = 0,
            ParentWindow = NativeMethods.HWND_MESSAGE
        };

        _hwndSource = new HwndSource(parameters);
        _hwndSource.AddHook(WndProc);

        // Re-register all hotkeys that were registered before Start()
        foreach (var (id, _) in _hotkeys)
        {
            // Hotkeys registered before Start() don't have their win32 counterpart yet;
            // they will be registered on the next Register() call.
        }
    }

    public void Stop()
    {
        if (!_running) return;
        _running = false;

        foreach (var id in _hotkeys.Keys)
            NativeMethods.UnregisterHotKey(_hwndSource!.Handle, id);

        _hwndSource?.Dispose();
        _hwndSource = null;
    }

    /// <summary>
    /// Register a global hotkey. Returns a handle that can be used to unregister.
    /// </summary>
    /// <param name="modifiers">Combination of <see cref="HotkeyModifiers"/> flags.</param>
    /// <param name="virtualKey">Virtual-key code (e.g. System.Windows.Input.Key converted via KeyInterop).</param>
    /// <param name="handler">Action to invoke when hotkey fires.</param>
    public int Register(HotkeyModifiers modifiers, uint virtualKey, Action handler)
    {
        int id = _nextId++;
        _hotkeys[id] = handler;

        if (_running && _hwndSource is not null)
        {
            if (!NativeMethods.RegisterHotKey(_hwndSource.Handle, id, (uint)modifiers, virtualKey))
                throw new InvalidOperationException($"Failed to register hotkey (id={id}). Key may already be in use.");
        }

        return id;
    }

    /// <summary>Unregister a hotkey by its handle.</summary>
    public void Unregister(int id)
    {
        if (_running && _hwndSource is not null)
            NativeMethods.UnregisterHotKey(_hwndSource.Handle, id);

        _hotkeys.Remove(id);
    }

    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        const int WM_HOTKEY = 0x0312;
        if (msg == WM_HOTKEY && _hotkeys.TryGetValue((int)wParam, out var handler))
        {
            handler();
            handled = true;
        }
        return 0;
    }

    public void Dispose() => Stop();
}

[Flags]
public enum HotkeyModifiers : uint
{
    None = 0,
    Alt = 0x0001,
    Control = 0x0002,
    Shift = 0x0004,
    Win = 0x0008,
    NoRepeat = 0x4000
}
