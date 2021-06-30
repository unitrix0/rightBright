using System;
using System.Runtime.InteropServices;
using unitrix0.rightbright.Services.MonitorAPI.Flags;
using unitrix0.rightbright.Services.MonitorAPI.Structs;

namespace unitrix0.rightbright.Services.WindowMessageApi
{
    public class WindowMessageApiImports
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle, [MarshalAs(UnmanagedType.LPStr)] string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern short RegisterClassEx([In] ref WndClassEx lpwcx);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
    }
}