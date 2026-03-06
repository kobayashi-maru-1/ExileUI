using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using ExileUI.Infrastructure;
using Timer = System.Threading.Timer;

namespace ExileUI.Core;

/// <summary>
/// Detects and tracks the Path of Exile game client window.
/// Equivalent to AHK's WinExist("ahk_class POEWindowClass") logic.
/// </summary>
public class GameClient
{
    private readonly AppState _state;
    private readonly Timer _pollTimer;

    private const string PoeWindowClass = "POEWindowClass";
    private const string GeForceNowExe = "GeForceNOW.exe";

    public event Action? ClientFound;
    public event Action? ClientLost;
    public event Action? ClientFocused;
    public event Action? ClientUnfocused;

    public GameClient(AppState state)
    {
        _state = state;
        _pollTimer = new Timer(Poll, null, Timeout.Infinite, Timeout.Infinite);
    }

    public void StartPolling() =>
        _pollTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(500));

    public void StopPolling() =>
        _pollTimer.Change(Timeout.Infinite, Timeout.Infinite);

    private void Poll(object? _)
    {
        var hwnd = FindPoeWindow();
        if (hwnd == 0)
        {
            if (_state.Client.Hwnd != 0)
            {
                _state.Client.Hwnd = 0;
                ClientLost?.Invoke();
            }
            return;
        }

        bool wasFound = _state.Client.Hwnd != 0;
        _state.Client.Hwnd = hwnd;

        if (!wasFound)
        {
            UpdateClientRect();
            DetectVersion();
            ClientFound?.Invoke();
        }

        bool isFocused = NativeMethods.GetForegroundWindow() == hwnd;
        if (isFocused != _state.Client.IsFocused)
        {
            _state.Client.IsFocused = isFocused;
            if (isFocused) ClientFocused?.Invoke();
            else ClientUnfocused?.Invoke();
        }

        UpdateClientRect();
    }

    private nint FindPoeWindow()
    {
        // Try native PoE window class first
        var hwnd = NativeMethods.FindWindowByClass(PoeWindowClass);
        if (hwnd != 0) return hwnd;

        // Fall back to GeForce NOW
        return FindProcessWindow(GeForceNowExe);
    }

    private static nint FindProcessWindow(string exeName)
    {
        var proc = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exeName)).FirstOrDefault();
        return proc?.MainWindowHandle ?? 0;
    }

    private void UpdateClientRect()
    {
        if (_state.Client.Hwnd == 0) return;
        if (!NativeMethods.GetClientRect(_state.Client.Hwnd, out var rect)) return;

        NativeMethods.ClientToScreen(_state.Client.Hwnd, out var topLeft);
        _state.Client.X = topLeft.X;
        _state.Client.Y = topLeft.Y;
        _state.Client.Width = rect.Right - rect.Left;
        _state.Client.Height = rect.Bottom - rect.Top;
    }

    private void DetectVersion()
    {
        // PoE 2 uses a different executable name
        var proc = Process.GetProcesses()
            .FirstOrDefault(p => p.MainWindowHandle == _state.Client.Hwnd);
        if (proc is null) return;

        _state.Client.IsPoe2 = proc.ProcessName.Contains("PathOfExile2", StringComparison.OrdinalIgnoreCase);
        _state.Client.IsGeForceNow = proc.ProcessName.Contains("GeForceNOW", StringComparison.OrdinalIgnoreCase);
    }
}
