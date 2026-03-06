using System.Windows;
using ExileUI.Overlays;

namespace ExileUI.Infrastructure;

/// <summary>
/// Creates, tracks, and destroys overlay windows.
/// Equivalent to AHK's vars.hwnd tracking object and LLK_Overlay() helper.
/// </summary>
public class WindowManager
{
    private readonly Dictionary<string, OverlayWindow> _windows = new();

    public OverlayWindow Show(string key, Action<OverlayWindow>? configure = null)
    {
        if (_windows.TryGetValue(key, out var existing))
        {
            existing.Show();
            return existing;
        }

        var window = new OverlayWindow();
        configure?.Invoke(window);
        _windows[key] = window;
        window.Show();
        return window;
    }

    public void Hide(string key)
    {
        if (_windows.TryGetValue(key, out var w)) w.Hide();
    }

    public void Destroy(string key)
    {
        if (_windows.TryGetValue(key, out var w))
        {
            w.Close();
            _windows.Remove(key);
        }
    }

    public void DestroyAll()
    {
        foreach (var key in _windows.Keys.ToList())
            Destroy(key);
    }

    public bool Exists(string key) =>
        _windows.TryGetValue(key, out var w) && w.IsVisible;

    public OverlayWindow? Get(string key) =>
        _windows.GetValueOrDefault(key);
}
