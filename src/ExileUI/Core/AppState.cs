namespace ExileUI.Core;

/// <summary>
/// Central shared state, equivalent to AHK's global `vars` object.
/// Modules read and write to this via well-typed sub-states.
/// </summary>
public class AppState
{
    public GameClientState Client { get; } = new();
    public LogState Log { get; } = new();
    public MouseState Mouse { get; } = new();
    public SystemState System { get; } = new();

    // Per-module states are added as modules are implemented
}

public class GameClientState
{
    public nint Hwnd { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsPoe2 { get; set; }
    public bool IsGeForceNow { get; set; }
    public bool IsFocused { get; set; }
}

public class LogState
{
    public string AreaId { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public int CharacterLevel { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public DateTime LastAreaChange { get; set; }
}

public class MouseState
{
    public int X { get; set; }
    public int Y { get; set; }
    public nint WindowUnderCursor { get; set; }
    public nint ControlUnderCursor { get; set; }
}

public class SystemState
{
    public int Timeout { get; set; }
    public bool IsDragging { get; set; }
}
