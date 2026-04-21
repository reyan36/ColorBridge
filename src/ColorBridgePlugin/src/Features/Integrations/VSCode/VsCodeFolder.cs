namespace Loupedeck.ColorBridgePlugin.Features.Integrations.VSCode
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class VsCodeFolder : PluginDynamicFolder
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public VsCodeFolder()
        {
            this.DisplayName = "VS Code Tools";
            this.Description = "Dynamic folder for VS Code integrations (CSS, SCSS, Tailwind)";
            this.GroupName = "6. VS Code Integrations";
        }

        public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _) => PluginDynamicFolderNavigation.ButtonArea;

        public override IEnumerable<String> GetButtonPressActionNames(DeviceType deviceType)
        {
            return new[]
            {
                this.CreateCommandName("css"),
                this.CreateCommandName("scss"),
                this.CreateCommandName("tailwind")
            };
        }

        public override void RunCommand(String actionParameter)
        {
            var palette = this._engine.Palette;
            if (palette == null || palette.Length == 0) return;

            var sb = new StringBuilder();
            var schemeName = this._engine.CurrentScheme.ToString().ToLower();

            if (actionParameter == "css")
            {
                sb.AppendLine(":root {");
                for (var i = 0; i < palette.Length; i++)
                {
                    var (h, s, l) = palette[i];
                    var hex = ColorConverter.HslToHex(h, s, l);
                    sb.AppendLine($"  --color-{schemeName}-{i + 1}: {hex};");
                }
                sb.Append("}");
                ClipboardService.CopyToClipboard(sb.ToString());
                PluginLog.Info("Exported palette as CSS variables to clipboard");
            }
            else if (actionParameter == "scss")
            {
                for (var i = 0; i < palette.Length; i++)
                {
                    var (h, s, l) = palette[i];
                    var hex = ColorConverter.HslToHex(h, s, l);
                    sb.AppendLine($"$color-{schemeName}-{(i + 1) * 100}: {hex};");
                }
                ClipboardService.CopyToClipboard(sb.ToString());
                PluginLog.Info("Exported palette as SCSS variables to clipboard");
            }
            else if (actionParameter == "tailwind")
            {
                sb.AppendLine("'" + schemeName + "': {");
                for (var i = 0; i < palette.Length; i++)
                {
                    var (h, s, l) = palette[i];
                    var hex = ColorConverter.HslToHex(h, s, l).ToLower();
                    var comma = i == palette.Length - 1 ? "" : ",";
                    sb.AppendLine($"  '{(i + 1) * 100}': '{hex}'{comma}");
                }
                sb.AppendLine("}");
                ClipboardService.CopyToClipboard(sb.ToString());
                PluginLog.Info("Exported palette as Tailwind JSON to clipboard");
            }
        }

        public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var svgName = actionParameter switch
            {
                "css" => "Loupedeck.ColorBridgePlugin.Features.Integrations.VSCode.ExportCssCommand.svg",
                "scss" => "Loupedeck.ColorBridgePlugin.Features.Integrations.VSCode.ExportScssCommand.svg",
                "tailwind" => "Loupedeck.ColorBridgePlugin.Features.Integrations.VSCode.ExportTailwindCommand.svg",
                _ => null
            };

            if (svgName != null)
            {
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
