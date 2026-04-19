namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class CopyHslCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public CopyHslCommand()
            : base(displayName: "Copy HSL", description: "Copy current color as HSL to clipboard", groupName: "Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            var hsl = ColorConverter.FormatAsHsl(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            ClipboardService.CopyToClipboard(hsl);
            PluginLog.Info($"Copied HSL: {hsl}");
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var (r, g, b) = ColorConverter.HslToRgb(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);

            using (var bmp = new BitmapBuilder(imageSize))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));
                bmp.DrawText("H", 0, 20, 90, 30, new BitmapColor((Byte)r, (Byte)g, (Byte)b), 28);
                bmp.DrawText("HSL", 0, 55, 90, 20, new BitmapColor(160, 160, 170), 12);
                return bmp.ToImage();
            }
        }
    }
}
