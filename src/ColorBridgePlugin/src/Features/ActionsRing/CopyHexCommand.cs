namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class CopyHexCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public CopyHexCommand()
            : base(displayName: "Copy HEX", description: "Copy current color as HEX to clipboard", groupName: "1. Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            var hex = ColorConverter.FormatAsHex(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            ClipboardService.CopyToClipboard(hex);
            PluginLog.Info($"Copied HEX: {hex}");
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
            => null; // Use SVG from actionsymbols folder
    }
}
