namespace Loupedeck.ColorBridgePlugin.Engine
{
    using System;

    public static class WcagChecker
    {
        public static Double ContrastRatio(Int32 r1, Int32 g1, Int32 b1, Int32 r2, Int32 g2, Int32 b2)
        {
            var lum1 = ColorConverter.RelativeLuminance(r1, g1, b1);
            var lum2 = ColorConverter.RelativeLuminance(r2, g2, b2);

            var lighter = Math.Max(lum1, lum2);
            var darker = Math.Min(lum1, lum2);

            return (lighter + 0.05) / (darker + 0.05);
        }

        public static Double ContrastRatioHsl(Int32 h1, Int32 s1, Int32 l1, Int32 h2, Int32 s2, Int32 l2)
        {
            var (r1, g1, b1) = ColorConverter.HslToRgb(h1, s1, l1);
            var (r2, g2, b2) = ColorConverter.HslToRgb(h2, s2, l2);
            return ContrastRatio(r1, g1, b1, r2, g2, b2);
        }

        public static Boolean PassesAA(Double contrastRatio) => contrastRatio >= 4.5;

        public static Boolean PassesAAA(Double contrastRatio) => contrastRatio >= 7.0;

        public static Boolean PassesAALargeText(Double contrastRatio) => contrastRatio >= 3.0;

        public static Boolean PassesAAALargeText(Double contrastRatio) => contrastRatio >= 4.5;

        public static ContrastResult CheckAgainstWhiteAndBlack(Int32 h, Int32 s, Int32 l)
        {
            var ratioVsWhite = ContrastRatioHsl(h, s, l, 0, 0, 100);
            var ratioVsBlack = ContrastRatioHsl(h, s, l, 0, 0, 0);

            return new ContrastResult
            {
                RatioVsWhite = Math.Round(ratioVsWhite, 2),
                RatioVsBlack = Math.Round(ratioVsBlack, 2),
                PassesAAOnWhite = PassesAA(ratioVsWhite),
                PassesAAAOnWhite = PassesAAA(ratioVsWhite),
                PassesAAOnBlack = PassesAA(ratioVsBlack),
                PassesAAAOnBlack = PassesAAA(ratioVsBlack)
            };
        }
    }

    public class ContrastResult
    {
        public Double RatioVsWhite { get; set; }
        public Double RatioVsBlack { get; set; }
        public Boolean PassesAAOnWhite { get; set; }
        public Boolean PassesAAAOnWhite { get; set; }
        public Boolean PassesAAOnBlack { get; set; }
        public Boolean PassesAAAOnBlack { get; set; }

        public String BestBackground => RatioVsWhite > RatioVsBlack ? "white" : "black";

        public String Summary
        {
            get
            {
                var bestRatio = Math.Max(RatioVsWhite, RatioVsBlack);
                var bg = BestBackground;
                var aa = (bg == "white" ? PassesAAOnWhite : PassesAAOnBlack) ? "✓" : "✗";
                var aaa = (bg == "white" ? PassesAAAOnWhite : PassesAAAOnBlack) ? "✓" : "✗";
                return $"AA{aa} AAA{aaa} {bestRatio}:1";
            }
        }
    }
}
