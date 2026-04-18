namespace Loupedeck.ColorBridgePlugin.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Versioning;

    [SupportedOSPlatform("windows")]
    internal static class ImagePaletteExtractor
    {
        public static Boolean TryExtractBaseColor(String imagePath, out (Int32 H, Int32 S, Int32 L) hsl)
        {
            hsl = (0, 100, 50);
            if (String.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                return false;

            try
            {
                using var bitmap = new Bitmap(imagePath);
                if (bitmap.Width <= 0 || bitmap.Height <= 0)
                    return false;

                var buckets = new Dictionary<Int32, (Double Score, Double H, Double S, Double L, Int32 Count)>();
                var stepX = Math.Max(1, bitmap.Width / 64);
                var stepY = Math.Max(1, bitmap.Height / 64);

                for (var y = 0; y < bitmap.Height; y += stepY)
                {
                    for (var x = 0; x < bitmap.Width; x += stepX)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        var (h, s, l) = ColorConverter.RgbToHsl(pixel.R, pixel.G, pixel.B);

                        // Skip very dark/light near-neutrals.
                        if (s < 8 || l < 5 || l > 95)
                            continue;

                        var bucket = h / 15; // 24 hue buckets
                        var vibrance = (s / 100.0) * (1.0 - (Math.Abs(l - 50.0) / 55.0));
                        var score = Math.Max(0.01, vibrance);

                        if (!buckets.TryGetValue(bucket, out var acc))
                            acc = (0, 0, 0, 0, 0);

                        buckets[bucket] = (
                            acc.Score + score,
                            acc.H + (h * score),
                            acc.S + (s * score),
                            acc.L + (l * score),
                            acc.Count + 1);
                    }
                }

                if (buckets.Count == 0)
                    return false;

                var bestBucket = -1;
                var bestScore = Double.MinValue;
                foreach (var pair in buckets)
                {
                    if (pair.Value.Score > bestScore)
                    {
                        bestScore = pair.Value.Score;
                        bestBucket = pair.Key;
                    }
                }

                if (bestBucket < 0)
                    return false;

                var best = buckets[bestBucket];
                var avgH = (Int32)Math.Round(best.H / best.Score);
                var avgS = (Int32)Math.Round(best.S / best.Score);
                var avgL = (Int32)Math.Round(best.L / best.Score);
                hsl = (((avgH % 360) + 360) % 360, Math.Clamp(avgS, 20, 100), Math.Clamp(avgL, 15, 85));
                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Warning($"Image extraction failed for '{imagePath}': {ex.Message}");
                return false;
            }
        }
    }
}
