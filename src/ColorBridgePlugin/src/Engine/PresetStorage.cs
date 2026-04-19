namespace Loupedeck.ColorBridgePlugin.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class PresetStorage
    {
        private static string _dataDir;
        private static string _filePath;
        private static List<SavedPreset> _userPresets = new List<SavedPreset>();

        public static IReadOnlyList<SavedPreset> UserPresets => _userPresets;

        public static void Init(string pluginDataDir)
        {
            _dataDir = pluginDataDir;
            _filePath = Path.Combine(_dataDir, "presets.txt");

            if (!Directory.Exists(_dataDir))
                Directory.CreateDirectory(_dataDir);

            Load();
        }

        public static void Save(int hue, int sat, int light, PaletteGenerator.SchemeType scheme)
        {
            var nextNum = _userPresets.Count + 1;
            var name = $"Preset {nextNum}";
            _userPresets.Add(new SavedPreset(name, hue, sat, light, scheme));
            WriteToDisk();
            PluginLog.Info($"Saved preset: {name} (H:{hue} S:{sat} L:{light} {scheme})");
        }

        public static void Delete(int index)
        {
            if (index < 0 || index >= _userPresets.Count) return;
            var name = _userPresets[index].Name;
            _userPresets.RemoveAt(index);
            WriteToDisk();
            PluginLog.Info($"Deleted preset: {name}");
        }

        private static void Load()
        {
            _userPresets.Clear();

            if (!File.Exists(_filePath))
                return;

            try
            {
                var lines = File.ReadAllLines(_filePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split('|');
                    if (parts.Length != 5) continue;

                    var name = parts[0];
                    if (!int.TryParse(parts[1], out var h)) continue;
                    if (!int.TryParse(parts[2], out var s)) continue;
                    if (!int.TryParse(parts[3], out var l)) continue;
                    if (!Enum.TryParse<PaletteGenerator.SchemeType>(parts[4], out var scheme)) continue;

                    _userPresets.Add(new SavedPreset(name, h, s, l, scheme));
                }

                PluginLog.Info($"Loaded {_userPresets.Count} user presets from disk");
            }
            catch (Exception ex)
            {
                PluginLog.Warning($"Failed to load presets: {ex.Message}");
            }
        }

        private static void WriteToDisk()
        {
            try
            {
                var lines = _userPresets.Select(p => $"{p.Name}|{p.Hue}|{p.Sat}|{p.Light}|{p.Scheme}");
                File.WriteAllLines(_filePath, lines);
            }
            catch (Exception ex)
            {
                PluginLog.Warning($"Failed to save presets: {ex.Message}");
            }
        }

        public class SavedPreset
        {
            public string Name { get; }
            public int Hue { get; }
            public int Sat { get; }
            public int Light { get; }
            public PaletteGenerator.SchemeType Scheme { get; }

            public SavedPreset(string name, int hue, int sat, int light, PaletteGenerator.SchemeType scheme)
            {
                Name = name;
                Hue = hue;
                Sat = sat;
                Light = light;
                Scheme = scheme;
            }
        }
    }
}
