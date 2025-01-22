using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using unitrix0.rightbright.Services.MonitorAPI.Structs;
using unitrix0.rightbright.Services.WindowMessageApi;

namespace unitrix0.rightbright.Services.MonitorAPI
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
