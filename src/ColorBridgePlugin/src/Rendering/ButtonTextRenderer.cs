namespace Loupedeck.ColorBridgePlugin.Rendering
{
    using System;

    /// <summary>
    /// Small helper for rendering plain text-only buttons with size-aware fonts.
    /// Mirrors the OBSStudioForLogi plugin pattern for readability on Width90 vs smaller sizes.
    /// </summary>
    public static class ButtonTextRenderer
    {
        public static BitmapImage RenderText(String text, PluginImageSize imageSize, BitmapColor? backgroundColor = null, BitmapColor? textColor = null)
        {
            var bg = backgroundColor ?? PluginTheme.BgDeep;
            var fg = textColor ?? PluginTheme.TextPrimary;
            var fontSize = GetFontSize(imageSize);

            using (var builder = new BitmapBuilder(imageSize))
            {
                builder.Clear(bg);
                builder.DrawText(text ?? String.Empty, fg, fontSize);
                return builder.ToImage();
            }
        }

        public static Int32 GetFontSize(PluginImageSize imageSize)
            => imageSize == PluginImageSize.Width90 ? 13 : 11;
    }
}
