namespace Loupedeck.ColorBridgePlugin.Platform
{
    using System;
    using System.Runtime.InteropServices;

    public static class ScreenColorPicker
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern Int32 ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern UInt32 GetPixel(IntPtr hDC, Int32 nXPos, Int32 nYPos);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public Int32 X;
            public Int32 Y;
        }

        public static (Int32 R, Int32 G, Int32 B)? GetColorAtCursor()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                PluginLog.Warning("Screen color picker is only supported on Windows.");
                return null;
            }

            try
            {
                if (!GetCursorPos(out var point))
                    return null;

                var hDC = GetDC(IntPtr.Zero);
                if (hDC == IntPtr.Zero)
                    return null;

                var pixel = GetPixel(hDC, point.X, point.Y);
                ReleaseDC(IntPtr.Zero, hDC);

                if (pixel == 0xFFFFFFFF)
                    return null;

                var r = (Int32)(pixel & 0xFF);
                var g = (Int32)((pixel >> 8) & 0xFF);
                var b = (Int32)((pixel >> 16) & 0xFF);

                return (r, g, b);
            }
            catch (DllNotFoundException ex)
            {
                PluginLog.Warning($"Screen picker: native DLL not yet available ({ex.Message}). Retrying later.");
                return null;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Screen picker error: {ex.Message}");
                return null;
            }
        }
    }
}
