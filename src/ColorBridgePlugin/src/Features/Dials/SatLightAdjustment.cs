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
            : base(displayName: "Sat / Light", description: "Rotate to adjust, press to toggle between saturation and lightness", groupName: "Page 4 — Dials", hasReset: true)
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
            var hue = this._engine.Hue;
            var sat = this._engine.Saturation;
            var light = this._engine.Lightness;
            var isSatMode = this._engine.CurrentSubDialMode == ColorEngine.SubDialMode.Saturation;

            var w = 90;
            using (var bmp = new BitmapBuilder(w, w))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));

                // --- Layout constants ---
                var stripX = 16;       // Left edge of strips (after label)
                var stripW = 66;       // Width of gradient strips
                var stripH = 6;        // Height of each strip
                var dotRadius = 4;     // Indicator dot radius

                // --- Saturation strip (row 1) ---
                var satY = 28;
                var satLabelColor = isSatMode ? new BitmapColor(255, 255, 255) : new BitmapColor(100, 100, 110);
                bmp.DrawText("S", 0, satY - 4, 14, 14, satLabelColor, 11);

                // Draw gradient: desaturated → fully saturated at current hue/lightness
                for (var i = 0; i < stripW; i++)
                {
                    var pct = (Int32)(i * 100.0 / stripW);
                    var (r, g, b) = ColorConverter.HslToRgb(hue, pct, 50);
                    bmp.FillRectangle(stripX + i, satY, 1, stripH, new BitmapColor((Byte)r, (Byte)g, (Byte)b));
                }

                // Indicator dot for saturation
                var satDotX = stripX + (Int32)(stripW * sat / 100.0);
                var satDotY = satY + stripH / 2;
                bmp.FillRectangle(satDotX - dotRadius, satDotY - dotRadius, dotRadius * 2, dotRadius * 2, new BitmapColor(255, 255, 255));
                // Inner dot with current color for contrast
                var (cr, cg, cb) = ColorConverter.HslToRgb(hue, sat, 50);
                bmp.FillRectangle(satDotX - 2, satDotY - 2, 4, 4, new BitmapColor((Byte)cr, (Byte)cg, (Byte)cb));

                // Sat value text (small, right-aligned)
                if (isSatMode)
                    bmp.DrawText($"{sat}%", stripX, satY + stripH + 2, stripW, 12, new BitmapColor(180, 180, 190), 9);

                // --- Lightness strip (row 2) ---
                var lightY = 54;
                var lightLabelColor = !isSatMode ? new BitmapColor(255, 255, 255) : new BitmapColor(100, 100, 110);
                bmp.DrawText("L", 0, lightY - 4, 14, 14, lightLabelColor, 11);

                // Draw gradient: black → white
                for (var i = 0; i < stripW; i++)
                {
                    var pct = (Int32)(i * 100.0 / stripW);
                    var (r, g, b) = ColorConverter.HslToRgb(hue, sat, pct);
                    bmp.FillRectangle(stripX + i, lightY, 1, stripH, new BitmapColor((Byte)r, (Byte)g, (Byte)b));
                }

                // Indicator dot for lightness
                var lightDotX = stripX + (Int32)(stripW * light / 100.0);
                var lightDotY = lightY + stripH / 2;
                bmp.FillRectangle(lightDotX - dotRadius, lightDotY - dotRadius, dotRadius * 2, dotRadius * 2, new BitmapColor(255, 255, 255));
                var (lr, lg, lb) = ColorConverter.HslToRgb(hue, sat, light);
                bmp.FillRectangle(lightDotX - 2, lightDotY - 2, 4, 4, new BitmapColor((Byte)lr, (Byte)lg, (Byte)lb));

                // Light value text
                if (!isSatMode)
                    bmp.DrawText($"{light}%", stripX, lightY + stripH + 2, stripW, 12, new BitmapColor(180, 180, 190), 9);

                // Active mode indicator — subtle underline on active strip label
                if (isSatMode)
                    bmp.FillRectangle(1, satY + 10, 12, 1, new BitmapColor(160, 130, 255));
                else
                    bmp.FillRectangle(1, lightY + 10, 12, 1, new BitmapColor(160, 130, 255));

                return bmp.ToImage();
            }
        }
    }
}
