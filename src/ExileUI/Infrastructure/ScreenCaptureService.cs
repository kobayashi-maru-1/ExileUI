using System.Drawing;
using System.Drawing.Imaging;
using ExileUI.Core;

namespace ExileUI.Infrastructure;

/// <summary>
/// Captures screen regions using GDI+ BitBlt.
/// Foundation for clone-frames (screen mirroring) and pixel/image search.
/// Equivalent to AHK's ImageSearch, PixelGetColor, and clone-frame screen reads.
/// </summary>
public class ScreenCaptureService
{
    private readonly AppState _state;

    public ScreenCaptureService(AppState state)
    {
        _state = state;
    }

    /// <summary>
    /// Capture a region of the screen into a Bitmap.
    /// </summary>
    public Bitmap CaptureRegion(int x, int y, int width, int height)
    {
        var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        g.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
        return bmp;
    }

    /// <summary>
    /// Capture the entire game client area.
    /// </summary>
    public Bitmap CaptureClient()
    {
        var c = _state.Client;
        return CaptureRegion(c.X, c.Y, c.Width, c.Height);
    }

    /// <summary>
    /// Get the color of a single screen pixel.
    /// </summary>
    public Color GetPixelColor(int x, int y)
    {
        using var bmp = CaptureRegion(x, y, 1, 1);
        return bmp.GetPixel(0, 0);
    }

    /// <summary>
    /// Search for a color within a region.
    /// Returns the first matching point, or null if not found.
    /// </summary>
    public Point? PixelSearch(int x, int y, int width, int height, Color target, int tolerance = 0)
    {
        using var bmp = CaptureRegion(x, y, width, height);
        var data = bmp.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int offset = row * data.Stride + col * 4;
                        byte b = ptr[offset];
                        byte g = ptr[offset + 1];
                        byte r = ptr[offset + 2];

                        if (Math.Abs(r - target.R) <= tolerance &&
                            Math.Abs(g - target.G) <= tolerance &&
                            Math.Abs(b - target.B) <= tolerance)
                        {
                            return new Point(x + col, y + row);
                        }
                    }
                }
            }
        }
        finally
        {
            bmp.UnlockBits(data);
        }

        return null;
    }

    /// <summary>
    /// Search for a bitmap image within a screen region.
    /// Returns top-left corner of first match, or null.
    /// </summary>
    public Point? ImageSearch(int x, int y, int width, int height, Bitmap needle, int tolerance = 0)
    {
        using var haystack = CaptureRegion(x, y, width, height);
        return FindBitmap(haystack, needle, tolerance) is Point found
            ? new Point(x + found.X, y + found.Y)
            : null;
    }

    private static Point? FindBitmap(Bitmap haystack, Bitmap needle, int tolerance)
    {
        int hw = haystack.Width, hh = haystack.Height;
        int nw = needle.Width, nh = needle.Height;
        if (nw > hw || nh > hh) return null;

        var hData = haystack.LockBits(new Rectangle(0, 0, hw, hh), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var nData = needle.LockBits(new Rectangle(0, 0, nw, nh), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            unsafe
            {
                byte* hp = (byte*)hData.Scan0;
                byte* np = (byte*)nData.Scan0;

                for (int hy = 0; hy <= hh - nh; hy++)
                for (int hx = 0; hx <= hw - nw; hx++)
                {
                    bool match = true;
                    for (int ny = 0; ny < nh && match; ny++)
                    for (int nx = 0; nx < nw && match; nx++)
                    {
                        int ho = (hy + ny) * hData.Stride + (hx + nx) * 4;
                        int no = ny * nData.Stride + nx * 4;
                        if (Math.Abs(hp[ho] - np[no]) > tolerance ||
                            Math.Abs(hp[ho+1] - np[no+1]) > tolerance ||
                            Math.Abs(hp[ho+2] - np[no+2]) > tolerance)
                            match = false;
                    }
                    if (match) return new Point(hx, hy);
                }
            }
        }
        finally
        {
            haystack.UnlockBits(hData);
            needle.UnlockBits(nData);
        }

        return null;
    }
}
