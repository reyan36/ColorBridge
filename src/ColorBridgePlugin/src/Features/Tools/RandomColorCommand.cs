namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class RandomColorCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;
        private static readonly Random Rng = new Random();
        private int _animFrame = 0;
        private System.Timers.Timer _animTimer;

        public RandomColorCommand()
            : base(displayName: "Random Color", description: "Generate a random base color", groupName: "Page 1 — Tools")
        {
            this._engine.ColorChanged += () => this.ActionImageChanged();

            _animTimer = new System.Timers.Timer(40);
            _animTimer.Elapsed += (s, e) =>
            {
                _animFrame = (_animFrame + 1) % 100;
                this.ActionImageChanged();
            };
            _animTimer.Start();
        }

        protected override void RunCommand(String actionParameter)
        {
            var h = Rng.Next(0, 360);
            var s = Rng.Next(45, 101);
            var l = Rng.Next(25, 76);
            this._engine.SetColor(h, s, l);
            PluginLog.Info($"Random color set to {ColorConverter.HslToHex(h, s, l)}");
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var hex = ColorConverter.HslToHex(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            var (r, g, b) = ColorConverter.HslToRgb(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            var w = imageSize == PluginImageSize.Width90 ? 90 : 80;
            var color = new BitmapColor((Byte)r, (Byte)g, (Byte)b);

            using (var bmp = new BitmapBuilder(imageSize))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));

                    // ── CHANGE POSITION HERE ──────────────────────────
                    var offsetX =  15;   // negative = move left,  positive = move right
                    var offsetY =  20;  // negative = move up,     positive = move down
                    var cx = w / 2 + offsetX;
                    var cy = w / 2 + offsetY;
                    // ─────────────────────────────────────────────────

                var dotRadius   = 4;
                var orbitRadius = 28;
                var coreRadius  = 15;

                var angle = (_animFrame / 100.0) * 2 * Math.PI;
                var dotX  = (int)(cx + orbitRadius * Math.Cos(angle));
                var dotY  = (int)(cy + orbitRadius * Math.Sin(angle));

                // Trailing dots
                for (int t = 1; t <= 5; t++)
                {
                    var trailAngle = angle - (t * 0.18);
                    var tx    = (int)(cx + orbitRadius * Math.Cos(trailAngle));
                    var ty    = (int)(cy + orbitRadius * Math.Sin(trailAngle));
                    var alpha = (byte)(180 - (t * 30));
                    bmp.FillCircle(tx, ty, dotRadius - 1, new BitmapColor(r, g, b, alpha));
                }

                // Orbiting dot
                bmp.FillCircle(dotX, dotY, dotRadius, color);

                // Center core dot
                bmp.FillCircle(cx, cy, coreRadius, color);

                // Hex label
bmp.DrawText(hex, offsetX, cy + orbitRadius + 6, w, 16, new BitmapColor(200, 200, 200), 16);    
                return bmp.ToImage();
            }
        }
    }
}
