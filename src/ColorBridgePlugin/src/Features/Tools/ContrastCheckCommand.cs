namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;

    public class ContrastCheckCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public ContrastCheckCommand()
            : base(displayName: "Contrast Check", description: "Shows WCAG contrast rating", groupName: "2. Tools")
        {
            this._engine.ColorChanged += () => this.ActionImageChanged("");
            this._engine.ContrastBackgroundChanged += () => this.ActionImageChanged("");
        }

        protected override void RunCommand(String actionParameter)
        {
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var (br, bg, bb) = this._engine.GetContrastBackgroundRgb();
            var (fr, fg, fb) = ColorConverter.HslToRgb(this._engine.Hue, this._engine.Saturation, this._engine.Lightness);
            var ratio = WcagChecker.ContrastRatio(fr, fg, fb, br, bg, bb);
            var passesAA = WcagChecker.PassesAA(ratio);
            var passesAAA = WcagChecker.PassesAAA(ratio);

            var level = passesAAA ? "AAA" : passesAA ? "AA" : "FAIL";

            BitmapColor ratingColor;
            if (passesAAA)
                ratingColor = new BitmapColor(76, 175, 80);
            else if (passesAA)
                ratingColor = new BitmapColor(255, 193, 7);
            else
                ratingColor = new BitmapColor(244, 67, 54);

            var w = imageSize == PluginImageSize.Width90 ? 90 : 80;

            using (var bmp = new BitmapBuilder(imageSize))
            {
                // Background #0a0a0f
                bmp.Clear(new BitmapColor(10, 10, 15));

                // Vertically space out FAIL and ratio explicitly spanning proper width
                bmp.DrawText(level, 21, 40, w, 30, ratingColor, w == 90 ? 26 : 24);
                bmp.DrawText($"{ratio:0.0}:1", 20, 75, w, 20, ratingColor, w == 90 ? 26 : 24);


                return bmp.ToImage();
            }
        }
    }
}
