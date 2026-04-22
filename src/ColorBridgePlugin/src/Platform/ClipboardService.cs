namespace Loupedeck.ColorBridgePlugin.Platform
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    public static class ClipboardService
    {
        [DllImport("user32.dll")]
        private static extern Boolean OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern Boolean CloseClipboard();

        [DllImport("user32.dll")]
        private static extern Boolean EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardData(UInt32 uFormat, IntPtr hMem);

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(UInt32 uFormat);

        [DllImport("user32.dll")]
        private static extern Boolean IsClipboardFormatAvailable(UInt32 format);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalAlloc(UInt32 uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern Boolean GlobalUnlock(IntPtr hMem);

        private const UInt32 CF_UNICODETEXT = 13;
        private const UInt32 GMEM_MOVEABLE = 0x0002;

        public static Boolean CopyToClipboard(String text)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                PluginLog.Warning("Clipboard is only supported on Windows.");
                return false;
            }

            var thread = new Thread(() =>
            {
                CopyToClipboardInternal(text);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            return true;
        }

        public static String ReadTextFromClipboard()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return String.Empty;

            String result = String.Empty;
            var thread = new Thread(() =>
            {
                result = ReadTextFromClipboardInternal();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join(1000);
            return result ?? String.Empty;
        }

        private static Boolean CopyToClipboardInternal(String text)
        {
            try
            {
                if (!OpenClipboard(IntPtr.Zero))
                    return false;

                EmptyClipboard();

                var chars = text.ToCharArray();
                var byteCount = (chars.Length + 1) * 2;
                var hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)byteCount);

                if (hGlobal == IntPtr.Zero)
                {
                    CloseClipboard();
                    return false;
                }

                var target = GlobalLock(hGlobal);
                if (target == IntPtr.Zero)
                {
                    CloseClipboard();
                    return false;
                }

                Marshal.Copy(chars, 0, target, chars.Length);
                Marshal.WriteInt16(target, chars.Length * 2, 0);

                GlobalUnlock(hGlobal);
                SetClipboardData(CF_UNICODETEXT, hGlobal);
                CloseClipboard();

                return true;
            }
            catch (DllNotFoundException ex)
            {
                PluginLog.Warning($"Clipboard: native DLL not available ({ex.Message})");
                return false;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Clipboard error: {ex.Message}");
                return false;
            }
        }

        private static String ReadTextFromClipboardInternal()
        {
            try
            {
                if (!OpenClipboard(IntPtr.Zero))
                    return String.Empty;

                if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
                {
                    CloseClipboard();
                    return String.Empty;
                }

                var hData = GetClipboardData(CF_UNICODETEXT);
                if (hData == IntPtr.Zero)
                {
                    CloseClipboard();
                    return String.Empty;
                }

                var ptr = GlobalLock(hData);
                if (ptr == IntPtr.Zero)
                {
                    CloseClipboard();
                    return String.Empty;
                }

                var text = Marshal.PtrToStringUni(ptr) ?? String.Empty;
                GlobalUnlock(hData);
                CloseClipboard();
                return text;
            }
            catch (DllNotFoundException ex)
            {
                PluginLog.Warning($"Clipboard read: native DLL not available ({ex.Message})");
                return String.Empty;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Clipboard read error: {ex.Message}");
                return String.Empty;
            }
        }
    }
}
