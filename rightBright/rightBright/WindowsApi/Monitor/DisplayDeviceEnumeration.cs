using System.Runtime.InteropServices;
using rightBright.WindowsApi.Monitor.Structs;

namespace rightBright.WindowsApi.Monitor;

internal static class DisplayDeviceEnumeration
{
    public const uint EddFlagsNone = 0;
    private const uint EddGetDeviceInterfaceName = 1;

    /// <summary>
    /// Fills <paramref name="dev"/> for a monitor device name from <see cref="WindowsMonitorApiImports.GetMonitorInfo"/>.
    /// Tries <c>dwFlags=0</c> first (friendly <see cref="DISPLAY_DEVICE.DeviceString"/>); then
    /// <c>EDD_GET_DEVICE_INTERFACE_NAME</c> for paths that require it.
    /// </summary>
    public static bool TryGetDisplayDeviceForMonitor(string monitorDeviceName, out DISPLAY_DEVICE dev,
        out uint lastAttemptFlags, out int lastWin32Error)
    {
        dev = new DISPLAY_DEVICE();
        dev.cb = Marshal.SizeOf<DISPLAY_DEVICE>();
        lastAttemptFlags = EddFlagsNone;

        if (WindowsMonitorApiImports.EnumDisplayDevices(monitorDeviceName, 0, ref dev, EddFlagsNone))
        {
            lastWin32Error = 0;
            return true;
        }

        lastWin32Error = Marshal.GetLastWin32Error();

        dev = new DISPLAY_DEVICE();
        dev.cb = Marshal.SizeOf<DISPLAY_DEVICE>();
        lastAttemptFlags = EddGetDeviceInterfaceName;

        if (WindowsMonitorApiImports.EnumDisplayDevices(monitorDeviceName, 0, ref dev, EddGetDeviceInterfaceName))
        {
            lastWin32Error = 0;
            return true;
        }

        lastWin32Error = Marshal.GetLastWin32Error();
        return false;
    }

    public static string ModelNameOrFallback(in DISPLAY_DEVICE dev, string monitorDeviceName) =>
        string.IsNullOrWhiteSpace(dev.DeviceString) ? monitorDeviceName : dev.DeviceString;
}
