using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace unitrix0.rightbright.Monitors.MonitorAPI
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PHYSICAL_MONITOR
    {
        public IntPtr hPhysicalMonitor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }
}