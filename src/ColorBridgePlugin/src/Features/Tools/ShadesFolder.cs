namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;
    using Loupedeck.ColorBridgePlugin.Rendering;

    public class ShadesFolder : PluginDynamicFolder
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;
        private Int32 _lastCopiedSlot = -1;
        private DateTime _lastCopyAtUtc = DateTime.MinValue;

        public ShadesFolder()
        {
            this.DisplayName = "Shades";
            this.Description = "Generate and show shade scale from current color";
            this.GroupName = "Page 1 — Tools";

            this._engine.PaletteChanged += () => RefreshAllSlots();
            this._engine.FormatChanged += () => RefreshAllSlots();
        }

        public override bool Activate()
        {
            this._engine.GenerateShades();
            PluginLog.Info("Generated shade scale");
            return base.Activate();
        }

        public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _)
        {
            return PluginDynamicFolderNavigation.ButtonArea;
        }

        public override IEnumerable<String> GetButtonPressActionNames(DeviceType deviceType)
        {
            return Enumerable.Range(0, 9).Select(i => this.CreateCommandName((i + 1).ToString()));
        }

        public override void RunCommand(String actionParameter)
        {
            if (!Int32.TryParse(actionParameter, out var slotNumber)) return;
            var slotIndex = slotNumber - 1;
            if (slotIndex < 0 || slotIndex >= 9) return;

            var palette = this._engine.Palette;
            if (palette == null || slotIndex >= palette.Length) return;

            var (h, s, l) = palette[slotIndex];

            var colorString = this._engine.GetFormattedPaletteColor(slotIndex);
            ClipboardService.CopyToClipboard(colorString);
            this._lastCopiedSlot = slotIndex;
            this._lastCopyAtUtc = DateTime.UtcNow;
            PluginLog.Info($"Copied {colorString} from shade slot {slotNumber}");

            for (var i = 1; i <= 9; i++)
                this.CommandImageChanged(i.ToString());
        }

        public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (!Int32.TryParse(actionParameter, out var slotNumber)) return null;
            var slotIndex = slotNumber - 1;
            if (slotIndex < 0 || slotIndex >= 9) return null;

            var palette = this._engine.Palette;
            if (palette == null || slotIndex >= palette.Length) return null;

            var (h, s, l) = palette[slotIndex];
            var hex = ColorConverter.HslToHex(h, s, l);
            var isRecentCopy = slotIndex == this._lastCopiedSlot && (DateTime.UtcNow - this._lastCopyAtUtc).TotalSeconds <= 2.0;
            var overlay = isRecentCopy ? "COPIED" : hex;

            return SwatchRenderer.RenderSwatch(h, s, l, imageSize, overlay);
        }

        private void RefreshAllSlots()
        {
            this.ButtonActionNamesChanged();
            for (var i = 1; i <= 9; i++)
            {
                this.CommandImageChanged(i.ToString());
            }
        }
    }
}
