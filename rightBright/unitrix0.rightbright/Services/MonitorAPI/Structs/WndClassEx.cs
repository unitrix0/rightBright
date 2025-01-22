using System;
using System.Runtime.InteropServices;

namespace unitrix0.rightbright.Services.MonitorAPI.Structs
{
    public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WndClassEx
    {
        [MarshalAs(UnmanagedType.U4)] public int cbSize;
        [MarshalAs(UnmanagedType.U4)] public int style;
        public WndProc lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPStr)] public string lpszClassName;
        public IntPtr hIconSm;

        public static WndClassEx Build()
        {
            var nw = new WndClassEx { cbSize = Marshal.SizeOf(typeof(WndClassEx)) };
            return nw;
        }
    }
}
