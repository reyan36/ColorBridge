namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class ScreenPickerCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public ScreenPickerCommand()
            : base(displayName: "Screen Picker", description: "Pick the color under your cursor and copy it", groupName: "2. Tools")
        {
            this._engine.ColorChanged += () => this.ActionImageChanged("");
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

                // â†“â†“ CHANGE SQUARE POSITION HERE â†“â†“
                var squareSize = 70;
                var squareX    = 20;  // â† move left/right
                var squareY    = 30;  // â† move up/down
                // â†‘â†‘ CHANGE SQUARE POSITION HERE â†‘â†‘

                bmp.FillRectangle(squareX, squareY, squareSize, squareSize, color);

                // Static white border around square
                bmp.DrawRectangle(squareX, squareY, squareSize, squareSize, new BitmapColor(255, 255, 255, 120));

                return bmp.ToImage();
            }
        }
    }
}
