namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class ScreenPickerCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;
        private int _animFrame = 0;
        private System.Timers.Timer _animTimer;

        public ScreenPickerCommand()
            : base(displayName: "Screen Picker", description: "Pick the color under your cursor and copy it", groupName: "Page 1 — Tools")
        {
            this._engine.ColorChanged += () => this.ActionImageChanged("");

            _animTimer = new System.Timers.Timer(50);
            _animTimer.Elapsed += (s, e) => {
                _animFrame = (_animFrame + 2) % 100;
                this.ActionImageChanged();
            };
            _animTimer.Start();
        }

        protected override void RunCommand(String actionParameter)
        {
            var color = ScreenColorPicker.GetColorAtCursor();

            if (color.HasValue)
            {
                var (r, g, b) = color.Value;
                this._engine.SetColorFromRgb(r, g, b);

                var hex = ColorConverter.RgbToHex(r, g, b);
                PluginLog.Info($"Picked color {hex} from screen");

                var formatted = this._engine.GetFormattedColor();
                ClipboardService.CopyToClipboard(formatted);
            }
            else
            {
                PluginLog.Warning("Screen color pick failed");
            }

            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var (r, g, b) = ColorConverter.HslToRgb(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            var color = new BitmapColor((Byte)r, (Byte)g, (Byte)b);

            var w = imageSize == PluginImageSize.Width90 ? 90 : 80;

            using (var bmp = new BitmapBuilder(imageSize))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));

                // ↓↓ CHANGE SQUARE POSITION HERE ↓↓
                var squareSize = 70;
                var squareX    = 20;  // ← move left/right
                var squareY    = 30;  // ← move up/down
                // ↑↑ CHANGE SQUARE POSITION HERE ↑↑

                bmp.FillRectangle(squareX, squareY, squareSize, squareSize, color);

                var perimeter = squareSize * 4;
                var segLen    = 20;
                var pos1      = (_animFrame * perimeter / 100) % perimeter;
                var pos2      = (pos1 + perimeter / 2) % perimeter;

                for (int pass = 0; pass < 2; pass++)
                {
                    var pos = pass == 0 ? pos1 : pos2;

                    for (int i = 0; i < segLen; i++)
                    {
                        var p = (pos + i) % perimeter;
                        float px, py;

                        if (p < squareSize)          { px = squareX + p;                                 py = squareY; }
                        else if (p < squareSize * 2) { px = squareX + squareSize;                        py = squareY + (p - squareSize); }
                        else if (p < squareSize * 3) { px = squareX + squareSize - (p - squareSize * 2); py = squareY + squareSize; }
                        else                         { px = squareX;                                     py = squareY + squareSize - (p - squareSize * 3); }

                        var alpha = (byte)(120 + (i * 135 / segLen));
                        bmp.FillRectangle((int)px, (int)py, 2, 2, new BitmapColor(255, 255, 255, alpha));
                    }
                }

                return bmp.ToImage();
            }
        }
    }
}
