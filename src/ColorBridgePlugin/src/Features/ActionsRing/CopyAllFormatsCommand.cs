namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class CopyAllFormatsCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public CopyAllFormatsCommand()
            : base(displayName: "Copy All Formats", description: "Copy HEX, RGB, HSL, and CMYK to clipboard", groupName: "1. Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            var h = this._engine.Hue;
            var s = this._engine.Saturation;
            var l = this._engine.Lightness;

            var all = $"{ColorConverter.FormatAsHex(h, s, l)}\n{ColorConverter.FormatAsRgb(h, s, l)}\n{ColorConverter.FormatAsHsl(h, s, l)}\n{ColorConverter.FormatAsCmyk(h, s, l)}";
            ClipboardService.CopyToClipboard(all);
            PluginLog.Info($"Copied all formats to clipboard");
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
            => null;
    }
}
