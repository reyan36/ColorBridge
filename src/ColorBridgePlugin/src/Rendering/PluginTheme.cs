namespace Loupedeck.ColorBridgePlugin.Rendering
{
    /// <summary>
    /// Centralized visual palette for button rendering.
    /// Matches ColorBridge prototype design tokens from prototype.html.
    /// </summary>
    public static class PluginTheme
    {
        // Prototype tokens:
        // --bg: #0a0a0f, --surface: #141420, --surface-2: #1c1c2e
        // --text: #e8e8f0, --text-muted: #8888a0, --accent: #6366f1
        // --success: #22c55e, --warning: #f59e0b, --danger: #ef4444
        public static readonly BitmapColor BgDeep = new BitmapColor(10, 10, 15);         // #0a0a0f
        public static readonly BitmapColor BgSurface = new BitmapColor(20, 20, 32);      // #141420
        public static readonly BitmapColor BgSurface2 = new BitmapColor(28, 28, 46);     // #1c1c2e
        public static readonly BitmapColor Border = new BitmapColor(42, 42, 64);          // #2a2a40
        public static readonly BitmapColor TextPrimary = new BitmapColor(232, 232, 240);  // #e8e8f0
        public static readonly BitmapColor TextMuted = new BitmapColor(136, 136, 160);    // #8888a0
        public static readonly BitmapColor AccentPrimary = new BitmapColor(99, 102, 241); // #6366f1
        public static readonly BitmapColor AccentViolet = new BitmapColor(192, 132, 252); // #c084fc
        public static readonly BitmapColor Success = new BitmapColor(34, 197, 94);        // #22c55e
        public static readonly BitmapColor Warning = new BitmapColor(245, 158, 11);       // #f59e0b
        public static readonly BitmapColor Danger = new BitmapColor(239, 68, 68);         // #ef4444
    }
}
