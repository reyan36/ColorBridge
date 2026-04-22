namespace Loupedeck.ColorBridgePlugin.Engine
{
    using System;
    using System.Threading;

    public class ColorEngine
    {
        private static ColorEngine _instance;
        public static ColorEngine Instance => _instance ??= new ColorEngine();

        private Int32 _hue = 0;
        private Int32 _saturation = 100;
        private Int32 _lightness = 50;

        public Int32 Hue => this._hue;
        public Int32 Saturation => this._saturation;
        public Int32 Lightness => this._lightness;

        public enum ColorFormat { HEX, RGB, HSL, CMYK }

        private ColorFormat _activeFormat = ColorFormat.HEX;
        public ColorFormat ActiveFormat => this._activeFormat;

        private PaletteGenerator.SchemeType _scheme = PaletteGenerator.SchemeType.Single;
        public PaletteGenerator.SchemeType CurrentScheme => this._scheme;

        private (Int32 H, Int32 S, Int32 L)[] _palette = new (Int32, Int32, Int32)[9];
        public (Int32 H, Int32 S, Int32 L)[] Palette => this._palette;

        public enum SubDialMode { Saturation, Lightness }

        private SubDialMode _subDialMode = SubDialMode.Saturation;
        public SubDialMode CurrentSubDialMode => this._subDialMode;

        public enum ContrastBackground { White, Dark }

        private ContrastBackground _contrastBackground = ContrastBackground.White;
        public ContrastBackground CurrentContrastBackground => this._contrastBackground;

        public event Action ColorChanged;
        public event Action PaletteChanged;
        public event Action FormatChanged;
        public event Action SubDialModeChanged;
        public event Action ContrastBackgroundChanged;

        // Debounce timer to prevent flooding Logi Plugin Service with rapid event cascades
        private Timer _debounceTimer;
        private readonly Object _debounceLock = new Object();
        private const Int32 DebounceMs = 50;

        private ColorEngine()
        {
            RegeneratePalette();
        }

        public void SetHue(Int32 hue)
        {
            this._hue = ((hue % 360) + 360) % 360;
            OnColorChanged();
        }

        public void AdjustHue(Int32 delta)
        {
            SetHue(this._hue + delta);
        }

        public void SetSaturation(Int32 sat)
        {
            this._saturation = Math.Max(0, Math.Min(100, sat));
            OnColorChanged();
        }

        public void AdjustSaturation(Int32 delta)
        {
            SetSaturation(this._saturation + delta);
        }

        public void SetLightness(Int32 light)
        {
            this._lightness = Math.Max(0, Math.Min(100, light));
            OnColorChanged();
        }

        public void AdjustLightness(Int32 delta)
        {
            SetLightness(this._lightness + delta);
        }

        public void SetColor(Int32 h, Int32 s, Int32 l)
        {
            this._hue = ((h % 360) + 360) % 360;
            this._saturation = Math.Max(0, Math.Min(100, s));
            this._lightness = Math.Max(0, Math.Min(100, l));
            OnColorChanged();
        }

        public void SetColorFromRgb(Int32 r, Int32 g, Int32 b)
        {
            var (h, s, l) = ColorConverter.RgbToHsl(r, g, b);
            SetColor(h, s, l);
        }

        public void CycleFormat()
        {
            var values = (ColorFormat[])Enum.GetValues(typeof(ColorFormat));
            var idx = Array.IndexOf(values, this._activeFormat);
            this._activeFormat = values[(idx + 1) % values.Length];
            OnFormatChanged();
        }

        public void SetFormat(ColorFormat format)
        {
            this._activeFormat = format;
            OnFormatChanged();
        }

        public void ToggleSubDialMode()
        {
            this._subDialMode = this._subDialMode == SubDialMode.Saturation
                ? SubDialMode.Lightness
                : SubDialMode.Saturation;
            SubDialModeChanged?.Invoke();
        }

        public void ToggleContrastBackground()
        {
            this._contrastBackground = this._contrastBackground == ContrastBackground.White
                ? ContrastBackground.Dark
                : ContrastBackground.White;
            ContrastBackgroundChanged?.Invoke();
        }

        public (Int32 R, Int32 G, Int32 B) GetContrastBackgroundRgb()
        {
            // Dark background matches prototype (#1a1a2e), not pure black, so user sees realistic UI contrast.
            return this._contrastBackground == ContrastBackground.White
                ? (255, 255, 255)
                : (26, 26, 46);
        }

        public void CycleScheme()
        {
            this._scheme = PaletteGenerator.NextScheme(this._scheme);
            RegeneratePalette();
            OnPaletteChanged();
        }

        public void CycleSchemeReverse()
        {
            this._scheme = PaletteGenerator.PreviousScheme(this._scheme);
            RegeneratePalette();
            OnPaletteChanged();
        }

        public void SetScheme(PaletteGenerator.SchemeType scheme)
        {
            this._scheme = scheme;
            RegeneratePalette();
            OnPaletteChanged();
        }

        public void GenerateShades()
        {
            this._scheme = PaletteGenerator.SchemeType.Shades;
            RegeneratePalette();
            OnPaletteChanged();
        }

        public void GenerateTints()
        {
            this._scheme = PaletteGenerator.SchemeType.Tints;
            RegeneratePalette();
            OnPaletteChanged();
        }

        private void RegeneratePalette()
        {
            this._palette = PaletteGenerator.Generate(this._hue, this._saturation, this._lightness, this._scheme);
        }

        public String GetFormattedColor() => FormatColor(this._hue, this._saturation, this._lightness);

        public String GetFormattedPaletteColor(Int32 slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= this._palette.Length)
                return "";

            var (h, s, l) = this._palette[slotIndex];
            return FormatColor(h, s, l);
        }

        public String GetPaletteSlotHex(Int32 slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= this._palette.Length)
                return "#000000";

            var (h, s, l) = this._palette[slotIndex];
            return ColorConverter.HslToHex(h, s, l);
        }

        private String FormatColor(Int32 h, Int32 s, Int32 l) => this._activeFormat switch
        {
            ColorFormat.HEX => ColorConverter.FormatAsHex(h, s, l),
            ColorFormat.RGB => ColorConverter.FormatAsRgb(h, s, l),
            ColorFormat.HSL => ColorConverter.FormatAsHsl(h, s, l),
            ColorFormat.CMYK => ColorConverter.FormatAsCmyk(h, s, l),
            _ => ColorConverter.FormatAsHex(h, s, l)
        };

        public static Boolean ShouldUseWhiteText(Int32 h, Int32 s, Int32 l)
        {
            var (r, g, b) = ColorConverter.HslToRgb(h, s, l);
            var luminance = ColorConverter.RelativeLuminance(r, g, b);
            return luminance < 0.4;
        }

        private void OnColorChanged()
        {
            RegeneratePalette();
            DebouncedNotify(() =>
            {
                ColorChanged?.Invoke();
                PaletteChanged?.Invoke();
            });
        }

        private void OnPaletteChanged()
        {
            DebouncedNotify(() => PaletteChanged?.Invoke());
        }

        private void OnFormatChanged()
        {
            DebouncedNotify(() => FormatChanged?.Invoke());
        }

        /// <summary>
        /// Debounce event notifications to prevent flooding the Logi Plugin Service thread.
        /// Without this, rapid state changes cause dozens of simultaneous image render requests
        /// that exceed the 1-second per-call timeout, freezing the entire plugin on startup.
        /// </summary>
        private void DebouncedNotify(Action callback)
        {
            lock (this._debounceLock)
            {
                this._debounceTimer?.Dispose();
                this._debounceTimer = new Timer(_ => callback(), null, DebounceMs, Timeout.Infinite);
            }
        }
    }
}
