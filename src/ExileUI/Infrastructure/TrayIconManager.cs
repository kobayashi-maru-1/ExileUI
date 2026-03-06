using System.IO;
using System.Windows.Forms;
using WpfApplication = System.Windows.Application;

namespace ExileUI.Infrastructure;

/// <summary>
/// System tray icon with context menu.
/// Equivalent to AHK's Menu, Tray setup.
/// </summary>
public class TrayIconManager : IDisposable
{
    private NotifyIcon? _notifyIcon;

    public void Initialize()
    {
        _notifyIcon = new NotifyIcon
        {
            Text = "Exile UI",
            Visible = true
        };

        // Load tray icon
        var iconPath = Path.Combine(AppContext.BaseDirectory, "img", "GUI", "tray.ico");
        if (File.Exists(iconPath))
            _notifyIcon.Icon = new System.Drawing.Icon(iconPath);

        var menu = new ContextMenuStrip();
        menu.Items.Add("Settings", null, (_, _) => OpenSettings());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => WpfApplication.Current.Shutdown());

        _notifyIcon.ContextMenuStrip = menu;
    }

    private static void OpenSettings()
    {
        // TODO: open settings window
    }

    public void Dispose()
    {
        _notifyIcon?.Dispose();
    }
}
