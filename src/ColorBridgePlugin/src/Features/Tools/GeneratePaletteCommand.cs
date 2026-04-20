namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class GeneratePaletteCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public GeneratePaletteCommand()
            : base(displayName: "Generate Palette", description: "Cycle scheme and regenerate palette", groupName: "2. Tools")
        {
            this._engine.PaletteChanged += () => this.ActionImageChanged();
            this._engine.FormatChanged  += () => this.ActionImageChanged();
        }

        protected override void RunCommand(String actionParameter)
        {
            this._engine.CycleScheme();
            PluginLog.Info($"Scheme is now: {this._engine.CurrentScheme}");
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var schemeName = this._engine.CurrentScheme switch
            {
                PaletteGenerator.SchemeType.Single             => "SINGLE",
                PaletteGenerator.SchemeType.Complementary      => "COMP",
                PaletteGenerator.SchemeType.Analogous          => "ANALOG",
                PaletteGenerator.SchemeType.Triadic            => "TRIAD",
                PaletteGenerator.SchemeType.SplitComplementary => "SPLIT",
                PaletteGenerator.SchemeType.Monochromatic      => "MONO",
                _                                              => "PALETTE"
            };

            var w = imageSize == PluginImageSize.Width90 ? 90 : 80;

            using (var bmp = new BitmapBuilder(imageSize))
            {
                bmp.Clear(new BitmapColor(10, 10, 15));

                // == 1. DYNAMIC ACTION SYMBOL ==
                // Draws three color swatches representing the current generated palette!
                // We pick three distinct slots from the 9-slot array to show the scheme range.
                if (this._engine.Palette != null && this._engine.Palette.Length >= 9)
                {
                    var (r1, g1, b1) = ColorConverter.HslToRgb(this._engine.Palette[0].H, this._engine.Palette[0].S, this._engine.Palette[0].L);
                    var (r2, g2, b2) = ColorConverter.HslToRgb(this._engine.Palette[4].H, this._engine.Palette[4].S, this._engine.Palette[4].L);
                    var (r3, g3, b3) = ColorConverter.HslToRgb(this._engine.Palette[8].H, this._engine.Palette[8].S, this._engine.Palette[8].L);

                    // â†“â†“ CHANGE CIRCLE POSITION & SIZE HERE â†“â†“
                    var circle1X = 29; // â† move left circle left/right
                    var circle2X = 55; // â† move middle circle left/right (45 = center)
                    var circle3X = 80; // â† move right circle left/right
                    var circleY  = 40; // â† move all circles up/down
                    var circleRadius = 15; // â† make bigger/smaller
                    // â†‘â†‘ CHANGE CIRCLE POSITION & SIZE HERE â†‘â†‘

                    bmp.FillCircle(circle1X, circleY, circleRadius, new BitmapColor((Byte)r1, (Byte)g1, (Byte)b1));
                    bmp.FillCircle(circle2X, circleY, circleRadius, new BitmapColor((Byte)r2, (Byte)g2, (Byte)b2));
                    bmp.FillCircle(circle3X, circleY, circleRadius, new BitmapColor((Byte)r3, (Byte)g3, (Byte)b3));
                }

                // == 2. MANUAL TEXT ALIGNMENT VARIABLES ==
                // â†“â†“ CHANGE SCHEME TEXT POSITION HERE â†“â†“
                var textX = 10;   // â† move left/right (0 = horizontally centered across textW)
                var textY = 80;  // â† move up/down
                
                // IMPORTANT: In Logi Options, textX + textW MUST be <= w (90). 
                // If it overflows 90, the text will turn completely blank!
                var textW = w;   // width bounding box
                var textH = 20;  // height bounding box 
                var fontSize = w == 90 ? 24 : 24; // font size
                // â†‘â†‘ CHANGE SCHEME TEXT POSITION HERE â†‘â†‘

                bmp.DrawText(schemeName, textX, textY, textW, textH, new BitmapColor(255, 255, 255), fontSize);

                return bmp.ToImage();
            }
        }
    }
}
