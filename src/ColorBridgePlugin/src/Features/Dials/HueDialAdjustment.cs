namespace Loupedeck.ColorBridgePlugin.Features.Dials
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;
    using Loupedeck.ColorBridgePlugin.Rendering;

    public class HueDialAdjustment : PluginDynamicAdjustment
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public HueDialAdjustment()
            : base(displayName: "Hue Wheel", description: "Rotate to sweep hue, press to reset", groupName: "5. Dials", hasReset: true)
        {
            this._engine.ColorChanged += () =>
            {
                this.AdjustmentValueChanged();
                this.ActionImageChanged("");
            };
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            this._engine.AdjustHue(diff * 3);
            this.AdjustmentValueChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            this._engine.SetHue(0);
            this.AdjustmentValueChanged();
            PluginLog.Info("Hue reset to 0");
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            return $"HUE {this._engine.Hue}deg  SAT {this._engine.Saturation}%  LIGHT {this._engine.Lightness}%";
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var h = this._engine.Hue;
            var w = 90; // Default canvas size
            using (var bmp = new BitmapBuilder(w, w))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));
                
                // Draw mode text vertically centered in the top half
                var textH = 30;
                var fontSize = 16;
                bmp.DrawText($"HUE {h}Â°", 0, 15, w, textH, new BitmapColor(255, 255, 255), fontSize);

                // Draw a dynamic progress bar at the bottom half
                var barWidth = 70;
                var barHeight = 8;
                var barX = (w - barWidth) / 2;
                var barY = 55;

                // Background track
                bmp.FillRectangle(barX, barY, barWidth, barHeight, new BitmapColor(40, 40, 50));

                // Foreground fill based on actual value
                var fillWidth = (Int32)(barWidth * (h / 359.0));
                
                // Emphasize the color so they see what they are dialing
                var (r, g, b) = ColorConverter.HslToRgb(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
                bmp.FillRectangle(barX, barY, fillWidth, barHeight, new BitmapColor((Byte)r, (Byte)g, (Byte)b));

                return bmp.ToImage();
            }
        }
    }
}
