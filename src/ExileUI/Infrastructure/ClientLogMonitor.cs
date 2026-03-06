using System.IO;
using ExileUI.Core;

namespace ExileUI.Infrastructure;

/// <summary>
/// Tails the Path of Exile client.txt log and fires events when relevant
/// lines are detected (area changes, level-ups, NPC dialogue, etc.).
/// Equivalent to AHK's client log module using file-read polling.
/// </summary>
public class ClientLogMonitor : IDisposable
{
    private readonly AppState _state;
    private readonly ConfigManager _config;

    private FileSystemWatcher? _watcher;
    private long _lastPosition;
    private string? _logPath;

    // --- Events ---
    public event Action<string>? AreaChanged;          // areaId
    public event Action<int>? LevelChanged;            // new level
    public event Action<string>? CharacterChanged;     // character name
    public event Action<string>? RawLineReceived;      // every new line

    // Known log patterns
    private static readonly string[] AreaChangeMarkers = ["] : You have entered ", "] : Wejście do "];
    private const string LevelUpMarker = "] : You have reached level ";
    private const string CharSelectMarker = "] Connecting to instance server at";

    public ClientLogMonitor(AppState state, ConfigManager config)
    {
        _state = state;
        _config = config;
    }

    public void Start()
    {
        _logPath = ResolveLogPath();
        if (_logPath is null || !File.Exists(_logPath)) return;

        // Seek to end — we only want new lines from here on
        _lastPosition = new FileInfo(_logPath).Length;

        var dir = Path.GetDirectoryName(_logPath)!;
        var file = Path.GetFileName(_logPath)!;

        _watcher = new FileSystemWatcher(dir, file)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true
        };
        _watcher.Changed += OnFileChanged;
    }

    public void Stop()
    {
        _watcher?.Dispose();
        _watcher = null;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (_logPath is null) return;

        try
        {
            using var fs = new FileStream(_logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fs.Seek(_lastPosition, SeekOrigin.Begin);

            using var reader = new StreamReader(fs);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                ProcessLine(line);
                RawLineReceived?.Invoke(line);
            }

            _lastPosition = fs.Position;
        }
        catch (IOException)
        {
            // File may be locked briefly — next change event will retry
        }
    }

    private void ProcessLine(string line)
    {
        // Area change
        foreach (var marker in AreaChangeMarkers)
        {
            int idx = line.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                string area = line[(idx + marker.Length)..].TrimEnd('.');
                _state.Log.AreaName = area;
                _state.Log.LastAreaChange = DateTime.Now;
                AreaChanged?.Invoke(area);
                return;
            }
        }

        // Level up
        {
            int idx = line.IndexOf(LevelUpMarker, StringComparison.Ordinal);
            if (idx >= 0)
            {
                string levelStr = line[(idx + LevelUpMarker.Length)..].Split(' ')[0];
                if (int.TryParse(levelStr, out int level))
                {
                    _state.Log.CharacterLevel = level;
                    LevelChanged?.Invoke(level);
                }
                return;
            }
        }
    }

    private string? ResolveLogPath()
    {
        // User-configured path takes priority
        var configured = _config.Get<string>("general", "log_path");
        if (!string.IsNullOrEmpty(configured) && File.Exists(configured))
            return configured;

        // Default Steam installation paths
        string[] candidates =
        [
            @"C:\Program Files (x86)\Steam\steamapps\common\Path of Exile\logs\Client.txt",
            @"C:\Program Files (x86)\Steam\steamapps\common\Path of Exile 2\logs\Client.txt",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Programs\Path of Exile\logs\Client.txt"),
        ];

        return candidates.FirstOrDefault(File.Exists);
    }

    public void Dispose() => Stop();
}
