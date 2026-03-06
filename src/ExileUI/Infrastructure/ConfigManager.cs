using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ExileUI.Infrastructure;

/// <summary>
/// Loads and saves per-feature configuration from JSON files in the ini/ folder.
/// Replaces AHK's IniRead/IniWrite with a JSON-based system.
/// </summary>
public class ConfigManager
{
    private readonly Dictionary<string, JsonObject> _cache = new();
    private const string ConfigDir = "ini";

    public void Load()
    {
        Directory.CreateDirectory(ConfigDir);
    }

    public T? Get<T>(string section, string key, T? defaultValue = default)
    {
        var obj = GetSection(section);
        if (obj is null || !obj.TryGetPropertyValue(key, out var node) || node is null)
            return defaultValue;

        return node.Deserialize<T>();
    }

    public void Set<T>(string section, string key, T value)
    {
        var obj = GetOrCreateSection(section);
        obj[key] = JsonValue.Create(value);
        SaveSection(section, obj);
    }

    private JsonObject? GetSection(string section)
    {
        if (_cache.TryGetValue(section, out var cached)) return cached;

        var path = SectionPath(section);
        if (!File.Exists(path)) return null;

        try
        {
            var text = File.ReadAllText(path);
            var obj = JsonSerializer.Deserialize<JsonObject>(text);
            if (obj is not null) _cache[section] = obj;
            return obj;
        }
        catch { return null; }
    }

    private JsonObject GetOrCreateSection(string section)
    {
        var obj = GetSection(section);
        if (obj is not null) return obj;
        obj = new JsonObject();
        _cache[section] = obj;
        return obj;
    }

    private void SaveSection(string section, JsonObject obj)
    {
        var path = SectionPath(section);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, obj.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    private static string SectionPath(string section) =>
        Path.Combine(ConfigDir, section + ".json");
}
