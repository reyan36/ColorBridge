namespace Loupedeck.ColorBridgePlugin.Rendering
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Renders a tool button by combining a reusable icon asset and text label.
    /// Follows the OBSStudioForLogi reference pattern: load embedded image,
    /// call BitmapBuilder.DrawImage directly, overlay optional text.
    /// </summary>
    public static class IconRenderer
    {
        private static readonly Dictionary<String, BitmapImage> IconCache = new Dictionary<String, BitmapImage>(StringComparer.OrdinalIgnoreCase);

        public static BitmapImage RenderImageOnly(String iconFileName)
            => LoadIcon(iconFileName);

        public static BitmapImage RenderToolButton(
            String iconFileName,
            String label,
            PluginImageSize imageSize)
            => RenderToolButton(iconFileName, label, imageSize, PluginTheme.BgDeep, PluginTheme.TextPrimary);

        public static BitmapImage RenderToolButton(
            String iconFileName,
            String label,
            PluginImageSize imageSize,
            BitmapColor backgroundColor,
            BitmapColor textColor)
        {
            using (var builder = new BitmapBuilder(imageSize))
            {
                builder.Clear(backgroundColor);

                var icon = LoadIcon(iconFileName);
                if (icon != null)
                {
                    try
                    {
                        builder.DrawImage(icon);
                    }
                    catch
                    {
                        // Keep rendering resilient; fallback path is label-only.
                    }
                }

                if (!String.IsNullOrWhiteSpace(label))
                {
                    var fontSize = ButtonTextRenderer.GetFontSize(imageSize);
                    builder.DrawText(label, textColor, fontSize);
                }

                return builder.ToImage();
            }
        }

        private static BitmapImage LoadIcon(String iconFileName)
        {
            if (String.IsNullOrWhiteSpace(iconFileName))
                return null;

            if (IconCache.TryGetValue(iconFileName, out var cached))
                return cached;

            try
            {
                var image = PluginResources.ReadImage(iconFileName);
                if (image != null)
                    IconCache[iconFileName] = image;

                return image;
            }
            catch (Exception ex)
            {
                // Do not cache failures: missing assets during dev would stick as null until restart.
                PluginLog.Warning($"Icon '{iconFileName}' not found: {ex.Message}");
                return null;
            }
        }
    }
}
