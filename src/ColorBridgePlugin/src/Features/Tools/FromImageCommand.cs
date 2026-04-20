namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;
    using Loupedeck.ColorBridgePlugin.Rendering;

    public class FromImageCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public FromImageCommand()
            : base(displayName: "From Image", description: "Extract palette from clipboard/image path", groupName: "2. Tools")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            if (!OperatingSystem.IsWindows())
            {
                PluginLog.Warning("From Image is currently supported on Windows only.");
                return;
            }

            var path = ResolveImagePath(actionParameter);
            if (String.IsNullOrWhiteSpace(path))
            {
                path = ShowImageFilePicker();
                if (String.IsNullOrWhiteSpace(path))
                {
                    PluginLog.Warning("From Image: no image selected.");
                    return;
                }
            }

            if (!ImagePaletteExtractor.TryExtractBaseColor(path, out var hsl))
            {
                PluginLog.Warning($"From Image: unable to extract color from '{path}'");
                return;
            }

            this._engine.SetColor(hsl.H, hsl.S, hsl.L);
            this._engine.SetScheme(PaletteGenerator.SchemeType.Analogous);
            PluginLog.Info($"From Image: palette extracted from '{Path.GetFileName(path)}' -> hsl({hsl.H}, {hsl.S}%, {hsl.L}%)");
        }

        private static String ResolveImagePath(String actionParameter)
        {
            if (IsSupportedImagePath(actionParameter))
                return actionParameter.Trim().Trim('"');

            var clipboardText = ClipboardService.ReadTextFromClipboard();
            if (IsSupportedImagePath(clipboardText))
                return clipboardText.Trim().Trim('"');

            return String.Empty;
        }

        private static Boolean IsSupportedImagePath(String path)
        {
            if (String.IsNullOrWhiteSpace(path))
                return false;

            var candidate = path.Trim().Trim('"');
            if (!File.Exists(candidate))
                return false;

            var ext = Path.GetExtension(candidate);
            return ext.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".webp", StringComparison.OrdinalIgnoreCase);
        }

        private static String ShowImageFilePicker()
        {
            try
            {
                // Use an STA PowerShell dialog to reliably prompt from plugin context.
                var script = @"
Add-Type -AssemblyName System.Windows.Forms
$dialog = New-Object System.Windows.Forms.OpenFileDialog
$dialog.Title = 'Select image for ColorBridge'
$dialog.Filter = 'Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp|All files|*.*'
$dialog.Multiselect = $false
$dialog.RestoreDirectory = $true
if ($dialog.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {
  [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
  Write-Output $dialog.FileName
}
";
                var psi = new ProcessStartInfo("powershell.exe")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };
                psi.ArgumentList.Add("-NoProfile");
                psi.ArgumentList.Add("-STA");
                psi.ArgumentList.Add("-ExecutionPolicy");
                psi.ArgumentList.Add("Bypass");
                psi.ArgumentList.Add("-Command");
                psi.ArgumentList.Add(script);

                using var process = Process.Start(psi);
                if (process == null)
                    return String.Empty;

                var exited = process.WaitForExit(30000);
                if (!exited)
                    return String.Empty;

                var output = process.StandardOutput.ReadToEnd().Trim();
                return IsSupportedImagePath(output) ? output.Trim().Trim('"') : String.Empty;
            }
            catch (Exception ex)
            {
                PluginLog.Warning($"From Image file picker failed: {ex.Message}");
                return String.Empty;
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            return IconRenderer.RenderImageOnly("From Image Icon.png")
                   ?? IconRenderer.RenderImageOnly("icon-image.png");
        }
    }
}
