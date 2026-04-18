namespace Loupedeck.ColorBridgePlugin.Features.Presets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;
    using Loupedeck.ColorBridgePlugin.Rendering;

    public class PresetsFolder : PluginDynamicFolder
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        private static readonly PresetEntry[] Presets =
        {
            new PresetEntry("Brand",    "Brand",    99, 102, 241, 239, 84, 67, PaletteGenerator.SchemeType.Single),
            new PresetEntry("Material", "Material", 244, 67, 54,  4,   82, 58, PaletteGenerator.SchemeType.Complementary),
            new PresetEntry("Pastel",   "Pastel",   232, 180, 248, 283, 45, 75, PaletteGenerator.SchemeType.Analogous),
            new PresetEntry("Earth",    "Earth",    141, 110, 99,  20,  18, 54, PaletteGenerator.SchemeType.Monochromatic),
            new PresetEntry("Neon",     "Neon",     57, 255, 20,  109, 97, 53, PaletteGenerator.SchemeType.Triadic),
            new PresetEntry("Ocean",    "Ocean",    0, 119, 182,  201, 75, 42, PaletteGenerator.SchemeType.Analogous),
            new PresetEntry("Sunset",   "Sunset",   255, 107, 53, 17, 100, 61, PaletteGenerator.SchemeType.SplitComplementary),
            new PresetEntry("Forest",   "Forest",   45, 106, 79,  149, 35, 36, PaletteGenerator.SchemeType.Monochromatic),
            new PresetEntry("New",      "+ New",     51, 51, 68,   239, 14, 26, PaletteGenerator.SchemeType.Single),
        };

        public PresetsFolder()
        {
            this.DisplayName = "Palette Presets";
            this.Description = "Dynamic folder of predefined palettes and a random generator.";
            this.GroupName = "Page 3 — Presets";

            this._engine.PaletteChanged += () => RefreshAllPresets();
            this._engine.ColorChanged += () => RefreshAllPresets();
        }

        public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _)
        {
            return PluginDynamicFolderNavigation.ButtonArea;
        }

        public override IEnumerable<String> GetButtonPressActionNames(DeviceType deviceType)
        {
            return Presets.Select(p => this.CreateCommandName(p.Id));
        }

        public override void RunCommand(String actionParameter)
        {
            var preset = FindPreset(actionParameter);
            if (preset == null) return;

            if (preset.Id == "new")
            {
                var rng = new Random();
                this._engine.SetColor(rng.Next(0, 360), rng.Next(50, 95), rng.Next(35, 65));
                this._engine.SetScheme(PaletteGenerator.SchemeType.Analogous);
                PluginLog.Info("New random palette created");
            }
            else
            {
                this._engine.SetColor(preset.Hue, preset.Sat, preset.Light);
                this._engine.SetScheme(preset.Scheme);
                PluginLog.Info($"Preset loaded: {preset.Label}");
            }
        }

        public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var preset = FindPreset(actionParameter);
            if (preset == null) return null;

            var useWhite = ColorEngine.ShouldUseWhiteText(preset.Hue, preset.Sat, preset.Light);
            var textColor = useWhite ? new BitmapColor(255, 255, 255) : new BitmapColor(0, 0, 0);

            using (var builder = new BitmapBuilder(imageSize))
            {
                builder.Clear(new BitmapColor(preset.R, preset.G, preset.B));
                builder.DrawText(preset.Label, textColor);
                return builder.ToImage();
            }
        }

        private static PresetEntry FindPreset(String id)
        {
            return string.IsNullOrEmpty(id) ? null : Presets.FirstOrDefault(p => p.Id == id);
        }

        private class PresetEntry
        {
            public readonly String Id;
            public readonly String Label;
            public readonly Byte R, G, B;
            public readonly Int32 Hue, Sat, Light;
            public readonly PaletteGenerator.SchemeType Scheme;

            public PresetEntry(String id, String label, Int32 r, Int32 g, Int32 b, Int32 h, Int32 s, Int32 l, PaletteGenerator.SchemeType scheme)
            {
                this.Id = id;
                this.Label = label;
                this.R = (Byte)r;
                this.G = (Byte)g;
                this.B = (Byte)b;
                this.Hue = h;
                this.Sat = s;
                this.Light = l;
                this.Scheme = scheme;
            }
        }

        private void RefreshAllPresets()
        {
            this.ButtonActionNamesChanged();
            foreach (var preset in Presets)
            {
                this.CommandImageChanged(preset.Id);
            }
        }
    }
}
