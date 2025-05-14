using System;
using System.Runtime.InteropServices;
using rightBright.WindowsApi.Monitor.Structs;

namespace rightBright.WindowsApi;

public class DxvaImports
{
    [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor,
        ref uint pdwNumberOfPhysicalMonitors);

    [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize,
        [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness,
        ref uint currentBrightness,
        ref uint maxBrightness);

    [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice,
        uint dwFlags);
}