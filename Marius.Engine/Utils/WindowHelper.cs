using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Marius.Utils
{
    internal class WindowHelper
    {
        public const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const uint WS_OVERLAPPED = 0x00000000;
        public const uint WS_POPUP = 0x80000000;
        public const uint WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
        public const uint WS_BORDER = 0x00800000;
        public const uint WS_SYSMENU = 0x00080000;
        public const uint WS_THICKFRAME = 0x00040000;

        public const uint WS_MINIMIZEBOX = 0x00020000;
        public const uint WS_MAXIMIZEBOX = 0x00010000;

        // Common Window Styles

        public const uint WS_OVERLAPPEDWINDOW =
            (WS_OVERLAPPED |
              WS_CAPTION |
              WS_SYSMENU |
              WS_THICKFRAME |
              WS_MINIMIZEBOX |
              WS_MAXIMIZEBOX);

        public const uint WS_POPUPWINDOW =
            (WS_POPUP |
              WS_BORDER |
              WS_SYSMENU);

        public const uint WS_EX_DLGMODALFRAME = 0x00000001;
        public const uint WS_EX_TOPMOST = 0x00000008;
        public const uint WS_EX_TOOLWINDOW = 0x00000080;
        public const uint WS_EX_WINDOWEDGE = 0x00000100;
        public const uint WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const uint WS_EX_LAYERED = 0x00080000;

        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_NOSENDCHANGING = 0x0400;

        public const uint WM_WINDOWPOSCHANGING = 0x0046;

        public const uint GW_HWNDNEXT = 2;

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        [DllImport("dwmapi.dll")]
        static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowEnabled(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public static RECT[] GetWindowRects()
        {
            List<RECT> rects = new List<RECT>();

            EnumWindows((IntPtr hwnd, IntPtr lParam) =>
            {
                uint style = GetWindowLong(hwnd, GWL_STYLE);
                uint exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

                bool isOverlapped = (style & WS_OVERLAPPEDWINDOW) == WS_OVERLAPPEDWINDOW;
                bool isPopup = (style & WS_POPUPWINDOW) == WS_POPUPWINDOW;
                bool isTool = (exStyle & WS_EX_TOOLWINDOW) == WS_EX_TOOLWINDOW;
                bool isPalette = (exStyle & WS_EX_PALETTEWINDOW) == WS_EX_PALETTEWINDOW;
                bool isDialog = (exStyle & WS_EX_DLGMODALFRAME) == WS_EX_DLGMODALFRAME;
                bool isLayered = (exStyle & WS_EX_LAYERED) == WS_EX_LAYERED;
                bool isAny = (isOverlapped || isPopup || isTool || isPalette || isDialog) && !isLayered;

                if (hwnd != GetDesktopWindow() && IsWindowEnabled(hwnd) && !IsIconic(hwnd) && IsWindowVisible(hwnd) && isAny)
                {
                    if (DwmGetWindowAttribute(hwnd, DWMWA_EXTENDED_FRAME_BOUNDS, out RECT rect, Marshal.SizeOf(typeof(RECT))) != 0)
                    {
                        GetWindowRect(hwnd, out rect);
                    }

                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        rects.Add(rect);
                    }
                }

                return true;
            }, IntPtr.Zero);

            return rects.ToArray();
        }
    }
}
