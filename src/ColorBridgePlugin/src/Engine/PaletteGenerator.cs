namespace Loupedeck.ColorBridgePlugin.Engine
{
    using System;

    public static class PaletteGenerator
    {
        public enum SchemeType
        {
            Single,
            Complementary,
            Analogous,
            Triadic,
            SplitComplementary,
            Monochromatic,
            Shades,
            Tints
        }

        public static (Int32 H, Int32 S, Int32 L)[] Generate(Int32 baseH, Int32 baseS, Int32 baseL, SchemeType scheme)
        {
            return scheme switch
            {
                SchemeType.Single => GenerateShades(baseH, baseS, baseL), // Legacy fallback
                SchemeType.Shades => GenerateShades(baseH, baseS, baseL),
                SchemeType.Tints => GenerateTints(baseH, baseS, baseL),
                SchemeType.Complementary => GenerateComplementary(baseH, baseS, baseL),
                SchemeType.Analogous => GenerateAnalogous(baseH, baseS, baseL),
                SchemeType.Triadic => GenerateTriadic(baseH, baseS, baseL),
                SchemeType.SplitComplementary => GenerateSplitComplementary(baseH, baseS, baseL),
                SchemeType.Monochromatic => GenerateMonochromatic(baseH, baseS, baseL),
                _ => GenerateShades(baseH, baseS, baseL)
            };
        }

        private static (Int32, Int32, Int32)[] GenerateShades(Int32 h, Int32 s, Int32 l)
        {
            var palette = new (Int32, Int32, Int32)[9];
            for (var i = 0; i < 9; i++)
            {
                var lightness = 10 + i * 10;
                palette[i] = (h, s, Clamp(lightness, 0, 100));
            }

            return palette;
        }

        private static (Int32, Int32, Int32)[] GenerateTints(Int32 h, Int32 s, Int32 l)
        {
            var palette = new (Int32, Int32, Int32)[9];
            for (var i = 0; i < 9; i++)
            {
                var newS = Math.Max(10, s - i * 6);
                var newL = Math.Min(95, 30 + i * 7);
                palette[i] = (h, newS, newL);
            }
            return palette;
        }

        private static (Int32, Int32, Int32)[] GenerateComplementary(Int32 h, Int32 s, Int32 l)
        {
            var comp = WrapHue(h + 180);
            return new (Int32, Int32, Int32)[]
            {
                (h, s, Clamp(l - 20, 5, 95)),
                (h, s, l),
                (h, s, Clamp(l + 20, 5, 95)),
                (h, Clamp(s - 15, 5, 100), l),
                (h, s, 50),
                (comp, Clamp(s - 15, 5, 100), l),
                (comp, s, Clamp(l - 20, 5, 95)),
                (comp, s, l),
                (comp, s, Clamp(l + 20, 5, 95))
            };
        }

        private static (Int32, Int32, Int32)[] GenerateAnalogous(Int32 h, Int32 s, Int32 l)
        {
            var h1 = WrapHue(h - 30);
            var h2 = h;
            var h3 = WrapHue(h + 30);

            return new (Int32, Int32, Int32)[]
            {
                (h1, s, Clamp(l - 15, 5, 95)),
                (h1, s, l),
                (h1, s, Clamp(l + 15, 5, 95)),
                (h2, s, Clamp(l - 15, 5, 95)),
                (h2, s, l),
                (h2, s, Clamp(l + 15, 5, 95)),
                (h3, s, Clamp(l - 15, 5, 95)),
                (h3, s, l),
                (h3, s, Clamp(l + 15, 5, 95))
            };
        }

        private static (Int32, Int32, Int32)[] GenerateTriadic(Int32 h, Int32 s, Int32 l)
        {
            var h1 = h;
            var h2 = WrapHue(h + 120);
            var h3 = WrapHue(h + 240);

            return new (Int32, Int32, Int32)[]
            {
                (h1, s, Clamp(l - 15, 5, 95)),
                (h1, s, l),
                (h1, s, Clamp(l + 15, 5, 95)),
                (h2, s, Clamp(l - 15, 5, 95)),
                (h2, s, l),
                (h2, s, Clamp(l + 15, 5, 95)),
                (h3, s, Clamp(l - 15, 5, 95)),
                (h3, s, l),
                (h3, s, Clamp(l + 15, 5, 95))
            };
        }

        private static (Int32, Int32, Int32)[] GenerateSplitComplementary(Int32 h, Int32 s, Int32 l)
        {
            var sc1 = WrapHue(h + 150);
            var sc2 = WrapHue(h + 210);

            return new (Int32, Int32, Int32)[]
            {
                (h, s, Clamp(l - 15, 5, 95)),
                (h, s, l),
                (h, s, Clamp(l + 15, 5, 95)),
                (sc1, s, Clamp(l - 15, 5, 95)),
                (sc1, s, l),
                (sc1, s, Clamp(l + 15, 5, 95)),
                (sc2, s, Clamp(l - 15, 5, 95)),
                (sc2, s, l),
                (sc2, s, Clamp(l + 15, 5, 95))
            };
        }

        private static (Int32, Int32, Int32)[] GenerateMonochromatic(Int32 h, Int32 s, Int32 l)
        {
            return new (Int32, Int32, Int32)[]
            {
                (h, Clamp(s - 30, 5, 100), 20),
                (h, Clamp(s - 15, 5, 100), 30),
                (h, s, 35),
                (h, Clamp(s - 10, 5, 100), 45),
                (h, s, 50),
                (h, Clamp(s - 10, 5, 100), 60),
                (h, s, 65),
                (h, Clamp(s - 15, 5, 100), 75),
                (h, Clamp(s - 30, 5, 100), 85)
            };
        }

        public static SchemeType NextScheme(SchemeType current)
        {
            var values = (SchemeType[])Enum.GetValues(typeof(SchemeType));
            var idx = Array.IndexOf(values, current);
            return values[(idx + 1) % values.Length];
        }

        public static SchemeType PreviousScheme(SchemeType current)
        {
            var values = (SchemeType[])Enum.GetValues(typeof(SchemeType));
            var idx = Array.IndexOf(values, current);
            return values[(idx - 1 + values.Length) % values.Length];
        }

        private static Int32 WrapHue(Int32 h) => ((h % 360) + 360) % 360;

        private static Int32 Clamp(Int32 value, Int32 min, Int32 max) =>
            Math.Max(min, Math.Min(max, value));
    }
}
