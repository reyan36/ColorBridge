namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class CopyHslCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public CopyHslCommand()
            : base(displayName: "Copy HSL", description: "Copy current color as HSL to clipboard", groupName: "1. Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            var hsl = ColorConverter.FormatAsHsl(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            ClipboardService.CopyToClipboard(hsl);
            PluginLog.Info($"Copied HSL: {hsl}");
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
            => null;
    }
}
