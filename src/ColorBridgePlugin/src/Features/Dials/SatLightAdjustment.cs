namespace Loupedeck.ColorBridgePlugin.Features.Dials
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;
    using Loupedeck.ColorBridgePlugin.Rendering;

    public class SatLightAdjustment : PluginDynamicAdjustment
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public SatLightAdjustment()
            : base(displayName: "Sat / Light", description: "Rotate to adjust, press to toggle between saturation and lightness", groupName: "5. Dials", hasReset: true)
        {
            this._engine.ColorChanged += () =>
            {
                this.AdjustmentValueChanged();
                this.ActionImageChanged();
            };
            this._engine.SubDialModeChanged += () =>
            {
                this.AdjustmentValueChanged();
                this.ActionImageChanged();
            };
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            if (this._engine.CurrentSubDialMode == ColorEngine.SubDialMode.Saturation)
            {
                this._engine.AdjustSaturation(diff * 2);
            }
            else
            {
                this._engine.AdjustLightness(diff * 2);
            }

            this.AdjustmentValueChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            this._engine.ToggleSubDialMode();
            this.AdjustmentValueChanged();

            var mode = this._engine.CurrentSubDialMode;
            PluginLog.Info($"Sub-dial mode: {mode}");
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            if (this._engine.CurrentSubDialMode == ColorEngine.SubDialMode.Saturation)
            {
                return $"SAT {this._engine.Saturation}%";
            }

            return $"LIGHT {this._engine.Lightness}%";
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var sat = this._engine.Saturation;
            var light = this._engine.Lightness;
            var isSatMode = this._engine.CurrentSubDialMode == ColorEngine.SubDialMode.Saturation;

            var (r, g, b) = ColorConverter.HslToRgb(this._engine.Hue, sat, light);

            var w = 90;
            using (var bmp = new BitmapBuilder(w, w))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));

                var activeColor = new BitmapColor(255, 255, 255);
                var dimColor = new BitmapColor(100, 100, 110);
                var accentColor = new BitmapColor(160, 130, 255);

                // Saturation row
                var satColor = isSatMode ? activeColor : dimColor;
                bmp.DrawText("SAT", 0, 22, w, 16, satColor, 13);
                bmp.DrawText($"{sat}%", 0, 38, w, 18, isSatMode ? accentColor : dimColor, isSatMode ? 18 : 14);

                // Lightness row
                var lightColor = !isSatMode ? activeColor : dimColor;
                bmp.DrawText("LIGHT", 0, 56, w, 16, lightColor, 13);
                bmp.DrawText($"{light}%", 0, 72, w, 18, !isSatMode ? accentColor : dimColor, !isSatMode ? 18 : 14);

                return bmp.ToImage();
            }
        }
    }
}
