namespace Loupedeck.ColorBridgePlugin.Features.Presets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class PresetsFolder : PluginDynamicFolder
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        // 5-click delete tracking
        private string _deleteTargetId = null;
        private int _deleteClickCount = 0;
        private DateTime _deleteStartTime = DateTime.MinValue;

        // Built-in presets (cannot be deleted)
        private static readonly BuiltInPreset[] BuiltIns =
        {
            new BuiltInPreset("brand",    "Brand",    99, 102, 241, 239, 84, 67, PaletteGenerator.SchemeType.Single),
            new BuiltInPreset("material", "Material", 244, 67, 54,  4,   82, 58, PaletteGenerator.SchemeType.Complementary),
            new BuiltInPreset("pastel",   "Pastel",   232, 180, 248, 283, 45, 75, PaletteGenerator.SchemeType.Analogous),
            new BuiltInPreset("earth",    "Earth",    141, 110, 99,  20,  18, 54, PaletteGenerator.SchemeType.Monochromatic),
            new BuiltInPreset("neon",     "Neon",     57, 255, 20,  109, 97, 53, PaletteGenerator.SchemeType.Triadic),
            new BuiltInPreset("ocean",    "Ocean",    0, 119, 182,  201, 75, 42, PaletteGenerator.SchemeType.Analogous),
            new BuiltInPreset("sunset",   "Sunset",   255, 107, 53, 17, 100, 61, PaletteGenerator.SchemeType.SplitComplementary),
            new BuiltInPreset("forest",   "Forest",   45, 106, 79,  149, 35, 36, PaletteGenerator.SchemeType.Monochromatic),
        };

        public PresetsFolder()
        {
            this.DisplayName = "Palette Presets";
            this.Description = "Built-in and saved palettes. Tap to load, 5x tap to delete saved ones.";
            this.GroupName = "4. Presets";
        }

        public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _)
        {
            return PluginDynamicFolderNavigation.ButtonArea;
        }

        public override IEnumerable<String> GetButtonPressActionNames(DeviceType deviceType)
        {
            // Built-ins first
            foreach (var b in BuiltIns)
                yield return this.CreateCommandName(b.Id);

            // User-saved presets
            for (var i = 0; i < PresetStorage.UserPresets.Count; i++)
                yield return this.CreateCommandName($"user_{i}");

            // Always show SAVE button at the end
            yield return this.CreateCommandName("__save__");
        }

        public override void RunCommand(String actionParameter)
        {
            if (string.IsNullOrEmpty(actionParameter)) return;

            // SAVE button
            if (actionParameter == "__save__")
            {
                PresetStorage.Save(
                    this._engine.Hue,
                    this._engine.Saturation,
                    this._engine.Lightness,
                    this._engine.CurrentScheme);

                this.ButtonActionNamesChanged();
                return;
            }

            // User preset â€” check for 5-click delete
            if (actionParameter.StartsWith("user_"))
            {
                if (int.TryParse(actionParameter.Substring(5), out var userIdx))
                {
                    // Delete logic: 5 rapid clicks within 3 seconds
                    var now = DateTime.UtcNow;
                    if (_deleteTargetId == actionParameter && (now - _deleteStartTime).TotalSeconds <= 3.0)
                    {
                        _deleteClickCount++;
                        if (_deleteClickCount >= 5)
                        {
                            PresetStorage.Delete(userIdx);
                            _deleteTargetId = null;
                            _deleteClickCount = 0;
                            this.ButtonActionNamesChanged();
                            return;
                        }
                        // Show countdown on button
                        this.CommandImageChanged(actionParameter);
                        return;
                    }
                    else
                    {
                        // First click or timeout â€” load the preset
                        _deleteTargetId = actionParameter;
                        _deleteClickCount = 1;
                        _deleteStartTime = now;

                        if (userIdx >= 0 && userIdx < PresetStorage.UserPresets.Count)
                        {
                            var p = PresetStorage.UserPresets[userIdx];
                            this._engine.SetColor(p.Hue, p.Sat, p.Light);
                            this._engine.SetScheme(p.Scheme);
                            PluginLog.Info($"Loaded user preset: {p.Name}");
                        }
                    }
                }
                return;
            }

            // Built-in preset
            var builtIn = BuiltIns.FirstOrDefault(b => b.Id == actionParameter);
            if (builtIn != null)
            {
                this._engine.SetColor(builtIn.Hue, builtIn.Sat, builtIn.Light);
                this._engine.SetScheme(builtIn.Scheme);
                PluginLog.Info($"Loaded built-in preset: {builtIn.Label}");
            }
        }

        public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (string.IsNullOrEmpty(actionParameter)) return null;

            // SAVE button
            if (actionParameter == "__save__")
            {
                using (var bmp = new BitmapBuilder(imageSize))
                {
                    bmp.Clear(new BitmapColor(30, 30, 40));
                    bmp.DrawText("+ SAVE", new BitmapColor(160, 130, 255), 16);
                    return bmp.ToImage();
                }
            }

            // User preset
            if (actionParameter.StartsWith("user_") && int.TryParse(actionParameter.Substring(5), out var idx))
            {
                if (idx < 0 || idx >= PresetStorage.UserPresets.Count) return null;
                var p = PresetStorage.UserPresets[idx];
                var (r, g, b) = ColorConverter.HslToRgb(p.Hue, p.Sat, p.Light);

                // Show delete countdown if actively clicking
                var deleteText = "";
                if (_deleteTargetId == actionParameter && _deleteClickCount > 0 && (DateTime.UtcNow - _deleteStartTime).TotalSeconds <= 3.0)
                {
                    var remaining = 5 - _deleteClickCount;
                    deleteText = $"X—{remaining}";
                }

                var useWhite = ColorEngine.ShouldUseWhiteText(p.Hue, p.Sat, p.Light);
                var textColor = useWhite ? new BitmapColor(255, 255, 255) : new BitmapColor(0, 0, 0);

                using (var bmp = new BitmapBuilder(imageSize))
                {
                    bmp.Clear(new BitmapColor((Byte)r, (Byte)g, (Byte)b));
                    if (!string.IsNullOrEmpty(deleteText))
                        bmp.DrawText(deleteText, new BitmapColor(255, 60, 60), 20);
                    else
                        bmp.DrawText(p.Name, textColor, 12);
                    return bmp.ToImage();
                }
            }

            // Built-in preset
            var preset = BuiltIns.FirstOrDefault(bi => bi.Id == actionParameter);
            if (preset == null) return null;

            var useW = ColorEngine.ShouldUseWhiteText(preset.Hue, preset.Sat, preset.Light);
            var tc = useW ? new BitmapColor(255, 255, 255) : new BitmapColor(0, 0, 0);

            using (var builder = new BitmapBuilder(imageSize))
            {
                builder.Clear(new BitmapColor(preset.R, preset.G, preset.B));
                builder.DrawText(preset.Label, tc);
                return builder.ToImage();
            }
        }

        private class BuiltInPreset
        {
            public readonly String Id, Label;
            public readonly Byte R, G, B;
            public readonly Int32 Hue, Sat, Light;
            public readonly PaletteGenerator.SchemeType Scheme;

            public BuiltInPreset(string id, string label, int r, int g, int b, int h, int s, int l, PaletteGenerator.SchemeType scheme)
            {
                Id = id; Label = label;
                R = (Byte)r; G = (Byte)g; B = (Byte)b;
                Hue = h; Sat = s; Light = l; Scheme = scheme;
            }
        }
    }
}
