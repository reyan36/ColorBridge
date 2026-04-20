namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using System.Text.RegularExpressions;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class PasteColorCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;
        private static readonly Regex HexPattern = new Regex(@"#?([0-9A-Fa-f]{6})", RegexOptions.Compiled);

        public PasteColorCommand()
            : base(displayName: "Paste Color", description: "Read a HEX color from clipboard and set it as current", groupName: "1. Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            var text = ClipboardService.ReadTextFromClipboard();
            if (string.IsNullOrWhiteSpace(text))
            {
                PluginLog.Warning("Paste Color: clipboard is empty");
                return;
            }

            var match = HexPattern.Match(text.Trim());
            if (!match.Success)
            {
                PluginLog.Warning($"Paste Color: no valid hex found in '{text}'");
                return;
            }

            var hex = match.Groups[1].Value;
            var (r, g, b) = ColorConverter.HexToRgb(hex);
            this._engine.SetColorFromRgb(r, g, b);
            PluginLog.Info($"Pasted color #{hex} from clipboard");
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
            => null;
    }
}
