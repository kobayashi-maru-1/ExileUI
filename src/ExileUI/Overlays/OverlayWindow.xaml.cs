using System.Windows;
using System.Windows.Interop;
using ExileUI.Infrastructure;

namespace ExileUI.Overlays;

/// <summary>
/// Base class for all overlay windows. Transparent, always-on-top, optionally click-through.
/// Equivalent to AHK's LLK_Overlay() pattern with WS_EX_LAYERED / WS_EX_TRANSPARENT.
/// </summary>
public partial class OverlayWindow : Window
{
    private bool _clickThrough;

    public bool ClickThrough
    {
        get => _clickThrough;
        set
        {
            _clickThrough = value;
            ApplyClickThrough();
        }
    }

    public OverlayWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        MakeToolWindow(hwnd);
        if (_clickThrough)
            ApplyClickThrough();
    }

    /// <summary>Removes the window from Alt+Tab and the taskbar.</summary>
    private static void MakeToolWindow(nint hwnd)
    {
        int exStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
        exStyle |= NativeMethods.WS_EX_TOOLWINDOW;
        exStyle &= ~NativeMethods.WS_EX_APPWINDOW;
        NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, exStyle);
    }

    /// <summary>
    /// Makes the window pass all mouse events through to the window beneath it.
    /// Set to false to allow interaction (e.g. clickable overlay panels).
    /// </summary>
    private void ApplyClickThrough()
    {
        if (!IsLoaded) return;
        var hwnd = new WindowInteropHelper(this).Handle;
        int exStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
        if (_clickThrough)
            exStyle |= NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED;
        else
            exStyle &= ~NativeMethods.WS_EX_TRANSPARENT;
        NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, exStyle);
    }

    /// <summary>Position the overlay at absolute screen coordinates.</summary>
    public void MoveTo(int x, int y)
    {
        Left = x;
        Top = y;
    }

    /// <summary>Resize the overlay.</summary>
    public void Resize(int width, int height)
    {
        Width = width;
        Height = height;
    }
}
