using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;

namespace WindowsTerminalQuake
{
    public class Toggler : IDisposable
    {
        private Process _process;
        private Process startedProc;
        private User32.Rect originalRect = default;

        public Toggler()
        {
            _process = Process.GetProcessesByName("WindowsTerminal").FirstOrDefault();
            if (_process == null)
            {
                startedProc = Process.Start("wt.exe");
                while(startedProc.MainWindowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(1);
                }
                startedProc.WaitForInputIdle();
                Thread.Sleep(1000);
                startedProc = Process.GetProcessesByName("WindowsTerminal").FirstOrDefault();
                _process = Process.GetProcessesByName("WindowsTerminal").FirstOrDefault();
            }

            // Store original position and size
            var proc = Process.GetProcessesByName("WindowsTerminal");
            var originalOk = User32.GetWindowRect(_process.MainWindowHandle, ref originalRect);
            // Hide from taskbar
            User32.SetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW);
            var style = User32.GetWindowLong(_process.MainWindowHandle, User32.GWL_STYLE);
            User32.SetWindowLong(_process.MainWindowHandle, User32.GWL_STYLE, (User32.GetWindowLong(_process.MainWindowHandle, User32.GWL_STYLE) & ~User32.WS_MAXIMIZEBOX & ~User32.WS_MINIMIZEBOX & ~User32.WS_THICKFRAME));
            var style_2 = User32.GetWindowLong(_process.MainWindowHandle, User32.GWL_STYLE);
            var bounds = GetScreenWithCursor().Bounds;
            User32.MoveWindow(_process.MainWindowHandle, bounds.Width/2-(originalRect.Right-originalRect.Left)/2, bounds.Y + (-(originalRect.Bottom - originalRect.Top)), (originalRect.Right - originalRect.Left), (originalRect.Bottom - originalRect.Top), true);


            User32.Rect rect = default;
            var ok = User32.GetWindowRect(_process.MainWindowHandle, ref rect);
            var isOpen = false;
            var isQuaked = true;

            var stepCount = 10;

            HotKeyManager.RegisterHotKey(Keys.Oemtilde, KeyModifiers.Control);
            HotKeyManager.RegisterHotKey(Keys.Q, KeyModifiers.Control);

            HotKeyManager.HotKeyPressed += (s, a) =>
            {
                if (a.Key == Keys.Q)
                {
                    Console.WriteLine("Switching Quake");
                }
                else
                {
                    if (isOpen)
                    {
                        isOpen = false;
                        Console.WriteLine("Close");

                        User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
                        User32.SetForegroundWindow(_process.MainWindowHandle);

                        bounds = GetScreenWithCursor().Bounds;

                        for (int i = stepCount - 1; i >= 0; i--)
                        {
                            User32.MoveWindow(_process.MainWindowHandle, bounds.Width / 2 - (originalRect.Right - originalRect.Left) / 2, bounds.Y + (-(originalRect.Bottom - originalRect.Top) + ((originalRect.Bottom - originalRect.Top) / stepCount * i)), originalRect.Right - originalRect.Left, originalRect.Bottom - originalRect.Top, true);
                            User32.SetWindowPos(_process.MainWindowHandle, User32.HWND_TOP, 0, 0, 0, 0, User32.SWP_NOMOVE + User32.SWP_NOSIZE + User32.SWP_HIDEWINDOW);
                            Task.Delay(1).GetAwaiter().GetResult();
                        }
                    }
                    else
                    {
                        isOpen = true;
                        Console.WriteLine("Open");

                        User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
                        User32.SetForegroundWindow(_process.MainWindowHandle);

                        bounds = GetScreenWithCursor().Bounds;

                        for (int i = 1; i <= stepCount; i++)
                        {
                            User32.MoveWindow(_process.MainWindowHandle, bounds.Width / 2 - (originalRect.Right - originalRect.Left) / 2, bounds.Y + (-(originalRect.Bottom - originalRect.Top) + ((originalRect.Bottom - originalRect.Top) / stepCount * i)), originalRect.Right - originalRect.Left, originalRect.Bottom - originalRect.Top, true);
                            User32.SetWindowPos(_process.MainWindowHandle, User32.HWND_TOPMOST, 0, 0, 0, 0, User32.SWP_NOMOVE + User32.SWP_NOSIZE + User32.SWP_SHOWWINDOW);
                            Task.Delay(1).GetAwaiter().GetResult();
                        }
                    }
                }
            };
        }

        public void Dispose()
        {
            ResetTerminal(_process, originalRect);
            startedProc.Close();
        }

        private static Screen GetScreenWithCursor()
        {
            return Screen.AllScreens.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position));
        }

        private static void ResetTerminal(Process process, User32.Rect originalRect)
        {
            var bounds = GetScreenWithCursor().Bounds;

            // Restore taskbar icon
            User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & User32.WS_EX_APPWINDOW);

            // Reset position
            User32.MoveWindow(process.MainWindowHandle, originalRect.Left, originalRect.Top, (originalRect.Right - originalRect.Left), (originalRect.Bottom - originalRect.Top), true);
        }
    }
}