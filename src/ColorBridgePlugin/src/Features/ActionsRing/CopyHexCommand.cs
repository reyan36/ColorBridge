namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class CopyHexCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public CopyHexCommand()
            : base(displayName: "Copy HEX", description: "Copy current color as HEX to clipboard", groupName: "Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            var hex = ColorConverter.FormatAsHex(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            ClipboardService.CopyToClipboard(hex);
            PluginLog.Info($"Copied HEX: {hex}");
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var (r, g, b) = ColorConverter.HslToRgb(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            var hex = ColorConverter.HslToHex(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);

            using (var bmp = new BitmapBuilder(imageSize))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));
                bmp.DrawText("#", 0, 20, 90, 30, new BitmapColor((Byte)r, (Byte)g, (Byte)b), 28);
                bmp.DrawText("HEX", 0, 55, 90, 20, new BitmapColor(160, 160, 170), 12);
                return bmp.ToImage();
            }
        }
    }
}
