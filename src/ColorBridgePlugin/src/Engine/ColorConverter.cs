namespace Loupedeck.ColorBridgePlugin.Engine
{
    using System;

    public static class ColorConverter
    {
        public static (Int32 R, Int32 G, Int32 B) HslToRgb(Int32 h, Int32 s, Int32 l)
        {
            var hNorm = h / 360.0;
            var sNorm = s / 100.0;
            var lNorm = l / 100.0;

            if (Math.Abs(sNorm) < 0.001)
            {
                var gray = (Int32)Math.Round(lNorm * 255);
                return (gray, gray, gray);
            }

            var q = lNorm < 0.5 ? lNorm * (1 + sNorm) : lNorm + sNorm - lNorm * sNorm;
            var p = 2 * lNorm - q;

            var r = HueToRgbChannel(p, q, hNorm + 1.0 / 3.0);
            var g = HueToRgbChannel(p, q, hNorm);
            var b = HueToRgbChannel(p, q, hNorm - 1.0 / 3.0);

            return (
                (Int32)Math.Round(r * 255),
                (Int32)Math.Round(g * 255),
                (Int32)Math.Round(b * 255)
            );
        }

        private static Double HueToRgbChannel(Double p, Double q, Double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
            return p;
        }

        public static (Int32 H, Int32 S, Int32 L) RgbToHsl(Int32 r, Int32 g, Int32 b)
        {
            var rNorm = r / 255.0;
            var gNorm = g / 255.0;
            var bNorm = b / 255.0;

            var max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
            var min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
            var delta = max - min;

            var lNorm = (max + min) / 2.0;
            Double hNorm = 0, sNorm = 0;

            if (Math.Abs(delta) > 0.001)
            {
                sNorm = lNorm > 0.5 ? delta / (2.0 - max - min) : delta / (max + min);

                if (Math.Abs(max - rNorm) < 0.001)
                    hNorm = ((gNorm - bNorm) / delta + (gNorm < bNorm ? 6 : 0)) / 6.0;
                else if (Math.Abs(max - gNorm) < 0.001)
                    hNorm = ((bNorm - rNorm) / delta + 2) / 6.0;
                else
                    hNorm = ((rNorm - gNorm) / delta + 4) / 6.0;
            }

            return (
                (Int32)Math.Round(hNorm * 360),
                (Int32)Math.Round(sNorm * 100),
                (Int32)Math.Round(lNorm * 100)
            );
        }

        public static String RgbToHex(Int32 r, Int32 g, Int32 b) => $"#{r:X2}{g:X2}{b:X2}";

        public static (Int32 R, Int32 G, Int32 B) HexToRgb(String hex)
        {
            hex = hex.TrimStart('#');
            return (
                Convert.ToInt32(hex.Substring(0, 2), 16),
                Convert.ToInt32(hex.Substring(2, 2), 16),
                Convert.ToInt32(hex.Substring(4, 2), 16)
            );
        }

        public static String HslToHex(Int32 h, Int32 s, Int32 l)
        {
            var (r, g, b) = HslToRgb(h, s, l);
            return RgbToHex(r, g, b);
        }

        public static (Int32 C, Int32 M, Int32 Y, Int32 K) RgbToCmyk(Int32 r, Int32 g, Int32 b)
        {
            if (r == 0 && g == 0 && b == 0)
                return (0, 0, 0, 100);

            var rNorm = r / 255.0;
            var gNorm = g / 255.0;
            var bNorm = b / 255.0;

            var k = 1 - Math.Max(rNorm, Math.Max(gNorm, bNorm));
            var c = (1 - rNorm - k) / (1 - k);
            var m = (1 - gNorm - k) / (1 - k);
            var y = (1 - bNorm - k) / (1 - k);

            return (
                (Int32)Math.Round(c * 100),
                (Int32)Math.Round(m * 100),
                (Int32)Math.Round(y * 100),
                (Int32)Math.Round(k * 100)
            );
        }

        public static (Int32 C, Int32 M, Int32 Y, Int32 K) HslToCmyk(Int32 h, Int32 s, Int32 l)
        {
            var (r, g, b) = HslToRgb(h, s, l);
            return RgbToCmyk(r, g, b);
        }

        public static String FormatAsHex(Int32 h, Int32 s, Int32 l) => HslToHex(h, s, l);

        public static String FormatAsRgb(Int32 h, Int32 s, Int32 l)
        {
            var (r, g, b) = HslToRgb(h, s, l);
            return $"rgb({r}, {g}, {b})";
        }

        public static String FormatAsHsl(Int32 h, Int32 s, Int32 l) => $"hsl({h}, {s}%, {l}%)";

        public static String FormatAsCmyk(Int32 h, Int32 s, Int32 l)
        {
            var (c, m, y, k) = HslToCmyk(h, s, l);
            return $"cmyk({c}%, {m}%, {y}%, {k}%)";
        }

        public static Double RelativeLuminance(Int32 r, Int32 g, Int32 b)
        {
            var rLin = LinearizeChannel(r / 255.0);
            var gLin = LinearizeChannel(g / 255.0);
            var bLin = LinearizeChannel(b / 255.0);
            return 0.2126 * rLin + 0.7152 * gLin + 0.0722 * bLin;
        }

        private static Double LinearizeChannel(Double c) =>
            c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
    }
}
