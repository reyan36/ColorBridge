namespace Loupedeck.ColorBridgePlugin.Features.Dials
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;
    using Loupedeck.ColorBridgePlugin.Rendering;

    public class SchemeAdjustment : PluginDynamicAdjustment
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public SchemeAdjustment()
            : base(displayName: "Palette Scheme", description: "Scroll to change palette scheme, press to reset", groupName: "Page 4 — Dials", hasReset: true)
        {
            this._engine.PaletteChanged += () =>
            {
                this.AdjustmentValueChanged();
                this.ActionImageChanged();
            };
            this._engine.ColorChanged += () =>
            {
                this.ActionImageChanged();
            };
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            if (diff > 0)
            {
                this._engine.CycleScheme();
            }
            else
            {
                this._engine.CycleSchemeReverse();
            }

            this.AdjustmentValueChanged();
            this.ActionImageChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            this._engine.SetScheme(PaletteGenerator.SchemeType.Single);
            this.AdjustmentValueChanged();
            this.ActionImageChanged();
            PluginLog.Info("Scheme reset to Single");
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            return GetReadableSchemeName(this._engine.CurrentScheme);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var scheme = this._engine.CurrentScheme;
            var schemeName = GetReadableSchemeName(scheme);
            var hue = this._engine.Hue;
            var sat = this._engine.Saturation;
            var light = this._engine.Lightness;

            var w = 90;
            using (var bmp = new BitmapBuilder(w, w))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));

                // Color accent bar at top
                var (r, g, b) = ColorConverter.HslToRgb(hue, sat, light);
                bmp.FillRectangle(0, 0, w, 5, new BitmapColor((Byte)r, (Byte)g, (Byte)b));

                // Scheme name centered
                bmp.DrawText(schemeName, 0, 28, w, 30, new BitmapColor(255, 255, 255), 14);

                return bmp.ToImage();
            }
        }

        private static String GetReadableSchemeName(PaletteGenerator.SchemeType scheme)
        {
            return scheme switch
            {
                PaletteGenerator.SchemeType.Single => "Single",
                PaletteGenerator.SchemeType.Complementary => "Compl.",
                PaletteGenerator.SchemeType.Analogous => "Analog.",
                PaletteGenerator.SchemeType.Triadic => "Triadic",
                PaletteGenerator.SchemeType.SplitComplementary => "Split C.",
                PaletteGenerator.SchemeType.Monochromatic => "Mono",
                PaletteGenerator.SchemeType.Shades => "Shades",
                PaletteGenerator.SchemeType.Tints => "Tints",
                _ => scheme.ToString()
            };
        }
    }
}
