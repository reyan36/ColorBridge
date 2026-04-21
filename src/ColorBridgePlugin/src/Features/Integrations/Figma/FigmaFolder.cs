namespace Loupedeck.ColorBridgePlugin.Features.Integrations.Figma
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class FigmaFolder : PluginDynamicFolder
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public FigmaFolder()
        {
            this.DisplayName = "Figma Tools";
            this.Description = "Dynamic folder for Figma integrations";
            this.GroupName = "7. Figma Integrations";
        }

        public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _) => PluginDynamicFolderNavigation.ButtonArea;

        public override IEnumerable<String> GetButtonPressActionNames(DeviceType deviceType)
        {
            return new[] { this.CreateCommandName("swatches") };
        }

        public override void RunCommand(String actionParameter)
        {
            if (actionParameter == "swatches")
            {
                var palette = this._engine.Palette;
                if (palette == null || palette.Length == 0) return;

                const int rectSize = 100;
                const int gap = 20;
                var width = (rectSize * palette.Length) + (gap * (palette.Length - 1));
                var height = rectSize + 60; 

                var sb = new StringBuilder();
                sb.AppendLine($"<svg width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {width} {height}\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\">");
                
                for (var i = 0; i < palette.Length; i++)
                {
                    var (h, s, l) = palette[i];
                    var hex = ColorConverter.HslToHex(h, s, l);
                    var x = i * (rectSize + gap);
                    sb.AppendLine($"  <rect x=\"{x}\" y=\"0\" width=\"{rectSize}\" height=\"{rectSize}\" fill=\"{hex}\" rx=\"8\" />");
                    sb.AppendLine($"  <text x=\"{x + (rectSize/2)}\" y=\"{rectSize + 30}\" font-family=\"Inter\" font-size=\"14\" fill=\"#333333\" text-anchor=\"middle\">{hex}</text>");
                }
                sb.AppendLine("</svg>");

                ClipboardService.CopyToClipboard(sb.ToString());
                PluginLog.Info("Exported Figma SVG swatches to clipboard");
            }
        }

        public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter == "swatches")
            {
                var svgName = "Loupedeck.ColorBridgePlugin.Features.Integrations.Figma.CopyFigmaSwatchesCommand.svg";
                try
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    using var stream = assembly.GetManifestResourceStream(svgName);
                    if (stream != null)
                    {
                        var buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        return BitmapImage.FromArray(buffer);
                    }
                }
                catch { }
            }
            return base.GetCommandImage(actionParameter, imageSize);
        }
    }
}
