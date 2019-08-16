using System;
using System.Runtime.InteropServices;

namespace WindowsTerminalQuake.Native
{
    public static class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, NCmdShow nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }


        public const int GWL_STYLE = -16, GWL_EX_STYLE = -20;
        public const int WS_CAPTION = 0x00C00000, WS_CLIPSIBLINGS = 0x04000000, WS_MAXIMIZEBOX = 0x00010000, WS_MINIMIZEBOX = 0x00020000, WS_THICKFRAME = 0x00040000;
        public const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
        public static IntPtr HWND_BOTTOM = new IntPtr(1), HWND_NOTOPMOST = new IntPtr(-2), HWND_TOP = new IntPtr(0), HWND_TOPMOST = new IntPtr(-1);
        public const UInt32 SWP_HIDEWINDOW = 0x0080, SWP_NOMOVE = 0x0002, SWP_NOSIZE = 0x0001, SWP_SHOWWINDOW = 0x0040;
    }
}