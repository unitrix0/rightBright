using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using rightBright.Services.SystemNotifications.Windows.Structs;
using rightBright.WindowsApi.WindowMessages;

namespace rightBright.Services.SystemNotifications.Windows
{
    public abstract class NotificationServiceBase : IDisposable
    {
        private WndClassEx _wndClass;

        protected void RegisterClass(string className)
        {
            _wndClass = WndClassEx.Build();
            _wndClass.lpfnWndProc = WindowProc;
            _wndClass.lpszClassName = className;

            if (WindowMessageApiImports.RegisterClassEx(ref _wndClass) == 0)
                Debug.WriteLine($"Failed to {nameof(RegisterClass)} '{className}': {Marshal.GetLastWin32Error()}");
        }

        protected abstract IntPtr WindowProc(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam);
        public abstract void Dispose();
    }
}
