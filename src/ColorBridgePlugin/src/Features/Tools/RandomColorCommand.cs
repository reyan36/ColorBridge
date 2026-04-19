namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class RandomColorCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;
        private static readonly Random Rng = new Random();

        // Store the last random color independently from the engine
        private int _lastH = -1;
        private int _lastS = -1;
        private int _lastL = -1;

        public RandomColorCommand()
            : base(displayName: "Random Color", description: "Generate a random base color", groupName: "Page 1 — Tools")
        {
            // Only update image when THIS command generates a new color, not on every engine change
        }

        protected override void RunCommand(String actionParameter)
        {
            var h = Rng.Next(0, 360);
            var s = Rng.Next(45, 101);
            var l = Rng.Next(25, 76);
            this._lastH = h;
            this._lastS = s;
            this._lastL = l;
            this._engine.SetColor(h, s, l);
            PluginLog.Info($"Random color set to {ColorConverter.HslToHex(h, s, l)}");
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var w = imageSize == PluginImageSize.Width90 ? 90 : 80;

            using (var bmp = new BitmapBuilder(imageSize))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));

                    // ── CHANGE POSITION HERE ──────────────────────────
                    var offsetX =  15;   // negative = move left,  positive = move right
                    var offsetY =  20;  // negative = move up,     positive = move down
                    var cx = w / 2 + offsetX;
                    var cy = w / 2 + offsetY;
                    // ─────────────────────────────────────────────────

                var orbitRadius = 28;
                var coreRadius  = 15;

                if (_lastH >= 0)
                {
                    // Show the last random color (not the current engine color)
                    var hex = ColorConverter.HslToHex(_lastH, _lastS, _lastL);
                    var (r, g, b) = ColorConverter.HslToRgb(_lastH, _lastS, _lastL);
                    var color = new BitmapColor((Byte)r, (Byte)g, (Byte)b);

                    // Center core dot
                    bmp.FillCircle(cx, cy, coreRadius, color);

                    // Hex label
                    bmp.DrawText(hex, offsetX, cy + orbitRadius + 6, w, 16, new BitmapColor(200, 200, 200), 16);
                }
                else
                {
                    // No random color generated yet — show placeholder
                    bmp.FillCircle(cx, cy, coreRadius, new BitmapColor(60, 60, 70));
                    bmp.DrawText("TAP", offsetX, cy + orbitRadius + 6, w, 16, new BitmapColor(120, 120, 130), 16);
                }

                return bmp.ToImage();
            }
        }
    }
}
