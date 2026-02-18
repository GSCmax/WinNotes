using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WinNotes.Helpers
{
    public static class WindowBackdropHelper
    {
        #region Public API
        public static void Apply(Window window)
        {
            window.SourceInitialized -= OnSourceInitialized;
            window.SourceInitialized += OnSourceInitialized;
        }

        private static void OnSourceInitialized(object? sender, EventArgs e)
        {
            if (sender is not Window window) return;

            if (IsWindows11())
                EnableWin11Backdrop(window);
            else
                EnableWin10Acrylic(window);
        }
        #endregion

        #region Windows Version
        private static bool IsWindows11()
        {
            // Win11 = build 22000+
            return Environment.OSVersion.Version.Build >= 22000;
        }

        private static bool IsSystemDark()
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            return (int?)key?.GetValue("AppsUseLightTheme") == 0;
        }
        #endregion

        #region Win11 Mica / Acrylic
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            int dwAttribute,
            ref int pvAttribute,
            int cbAttribute);

        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

        private enum DwmSystemBackdropType
        {
            Auto = 0,
            None = 1,
            MainWindow = 2,      // Mica
            TransientWindow = 3, // Acrylic
            TabbedWindow = 4
        }

        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        private static void EnableWin11Backdrop(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;

            var margins = new MARGINS()
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyTopHeight = -1,
                cyBottomHeight = -1
            };
            DwmExtendFrameIntoClientArea(hwnd, ref margins);

            int dark = IsSystemDark() ? 1 : 0;
            DwmSetWindowAttribute(hwnd, 20, ref dark, sizeof(int));

            int backdrop = (int)DwmSystemBackdropType.MainWindow;
            DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdrop, sizeof(int));
        }
        #endregion

        #region Win10 Acrylic (AccentPolicy)
        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        }

        private enum WindowCompositionAttribute { WCA_ACCENT_POLICY = 19 }

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        private static void EnableWin10Acrylic(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;

            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,

                // AARRGGBB (半透明黑)
                GradientColor = (120 << 24) | (0x1E << 16) | (0x1E << 8) | 0x1E
            };

            int size = Marshal.SizeOf(accent);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(accent, ptr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                Data = ptr,
                SizeOfData = size
            };

            SetWindowCompositionAttribute(hwnd, ref data);

            Marshal.FreeHGlobal(ptr);
        }
        #endregion
    }
}
