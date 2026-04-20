namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System.Text;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;
    using Loupedeck.ColorBridgePlugin.Rendering;

    public class SavePaletteCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;
        private bool _savedRecently = false;

        public SavePaletteCommand()
            : base(displayName: "Save Palette", description: "Copy full palette to clipboard", groupName: "2. Tools")
        {
        }

        protected override void RunCommand(string actionParameter)
        {
            var lines = new StringBuilder();
            for (var i = 0; i < 9; i++)
            {
                var hex = this._engine.GetPaletteSlotHex(i);
                lines.AppendLine($"Slot {i + 1}: {hex}");
            }

            if (ClipboardService.CopyToClipboard(lines.ToString().Trim()))
            {
                this._savedRecently = true;
                PluginLog.Info("Full palette copied to clipboard");
            }

            this.ActionImageChanged("");
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (this._savedRecently)
                this._savedRecently = false;

            return IconRenderer.RenderImageOnly("Save Palette.png")
                   ?? IconRenderer.RenderImageOnly("icon-save.png");
        }
    }
}
