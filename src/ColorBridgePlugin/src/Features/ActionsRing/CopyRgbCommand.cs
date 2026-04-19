namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class CopyRgbCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public CopyRgbCommand()
            : base(displayName: "Copy RGB", description: "Copy current color as RGB to clipboard", groupName: "Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            var rgb = ColorConverter.FormatAsRgb(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            ClipboardService.CopyToClipboard(rgb);
            PluginLog.Info($"Copied RGB: {rgb}");
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var (r, g, b) = ColorConverter.HslToRgb(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);

            using (var bmp = new BitmapBuilder(imageSize))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));
                bmp.DrawText("R", 0, 20, 90, 30, new BitmapColor((Byte)r, (Byte)g, (Byte)b), 28);
                bmp.DrawText("RGB", 0, 55, 90, 20, new BitmapColor(160, 160, 170), 12);
                return bmp.ToImage();
            }
        }
    }
}
