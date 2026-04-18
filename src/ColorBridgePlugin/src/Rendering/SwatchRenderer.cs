namespace Loupedeck.ColorBridgePlugin.Rendering
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;

    /// <summary>
    /// Renders dynamic color swatch images for the MX Creative Console LCD buttons.
    /// Each button image is 80x80 pixels showing the color fill + HEX code text.
    /// </summary>
    public static class SwatchRenderer
    {
        /// <summary>
        /// Creates a button image showing a solid color swatch with the HEX code overlaid.
        /// Uses the SDK's BitmapBuilder for rendering.
        /// </summary>
        public static BitmapImage RenderSwatch(Int32 h, Int32 s, Int32 l, PluginImageSize imageSize, String overlayText = null)
        {
            var hex = ColorConverter.HslToHex(h, s, l);
            var (r, g, b) = ColorConverter.HslToRgb(h, s, l);

            // Determine text color based on background luminance
            var useWhiteText = ColorEngine.ShouldUseWhiteText(h, s, l);
            var textColor = useWhiteText ? new BitmapColor(255, 255, 255) : new BitmapColor(0, 0, 0);

            using (var builder = new BitmapBuilder(imageSize))
            {
                // Fill with the actual color
                builder.Clear(new BitmapColor((Byte)r, (Byte)g, (Byte)b));

                // Draw the HEX code (or custom overlay text) on top
                var text = overlayText ?? hex;
                builder.DrawText(text, textColor);

                return builder.ToImage();
            }
        }

        /// <summary>
        /// Creates a button image with a label (e.g. format name, action name).
        /// </summary>
        public static BitmapImage RenderLabelButton(String label, BitmapColor bgColor, BitmapColor textColor, PluginImageSize imageSize)
        {
            using (var builder = new BitmapBuilder(imageSize))
            {
                builder.Clear(bgColor);
                builder.DrawText(label, textColor);
                return builder.ToImage();
            }
        }

        /// <summary>
        /// Creates a button image showing WCAG contrast result.
        /// </summary>
        public static BitmapImage RenderContrastButton(Int32 h, Int32 s, Int32 l, PluginImageSize imageSize)
        {
            var result = WcagChecker.CheckAgainstWhiteAndBlack(h, s, l);
            var bestRatio = Math.Max(result.RatioVsWhite, result.RatioVsBlack);
            var passesAAA = result.BestBackground == "white" ? result.PassesAAAOnWhite : result.PassesAAAOnBlack;
            var passesAA = result.BestBackground == "white" ? result.PassesAAOnWhite : result.PassesAAOnBlack;
            var label = passesAAA ? "AAA" : (passesAA ? "AA" : "FAIL");
            var textColor = passesAAA ? PluginTheme.Success : (passesAA ? PluginTheme.Warning : PluginTheme.Danger);

            using (var builder = new BitmapBuilder(imageSize))
            {
                builder.Clear(PluginTheme.BgDeep);
                builder.DrawText($"{label} {bestRatio:0.0}", textColor);
                return builder.ToImage();
            }
        }
    }
}
