using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using rightBright.WindowsApi.Monitor.Structs;

namespace rightBright.WindowsApi.Monitor;

internal static class DisplayDeviceEnumerationDiagnostics
{
    private const uint EddGetDeviceInterfaceName = 1;

    public static string FormatWin32Error(int err) =>
        err == 0
            ? "last Win32 error was 0 (API may not have called SetLastError on this failure path)"
            : $"{err} ({new Win32Exception(err).Message})";

    public static string FormatGetMonitorInfoFailure(IntPtr hMonitor, int err) =>
        $"Win32: {FormatWin32Error(err)}; hMonitor=0x{hMonitor.ToInt64():X}";

    public static string FormatEnumDisplayDevicesFailure(string monitorDeviceName, in DISPLAY_DEVICE dev, uint dwFlags,
        int err)
    {
        var expectedCb = Marshal.SizeOf<DISPLAY_DEVICE>();
        return
            $"Win32: {FormatWin32Error(err)}; monitorDeviceName={monitorDeviceName}; dev.cb={dev.cb}; expectedCb={expectedCb}; dwFlags=0x{dwFlags:X} ({DescribeEnumDisplayDevicesFlags(dwFlags)})";
    }

    private static string DescribeEnumDisplayDevicesFlags(uint dwFlags)
    {
        if (dwFlags == 0) return "none";
        var rest = dwFlags & ~EddGetDeviceInterfaceName;
        if ((dwFlags & EddGetDeviceInterfaceName) != 0)
            return rest == 0 ? "EDD_GET_DEVICE_INTERFACE_NAME" : $"EDD_GET_DEVICE_INTERFACE_NAME | 0x{rest:X}";
        return $"0x{dwFlags:X}";
    }
}
