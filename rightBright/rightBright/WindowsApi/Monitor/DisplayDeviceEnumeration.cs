using System;
using System.Runtime.InteropServices;
using rightBright.WindowsApi.Monitor.Flags;
using rightBright.WindowsApi.Monitor.Structs;

namespace rightBright.WindowsApi.Monitor;

internal static class DisplayDeviceEnumeration
{
    public const uint EddFlagsNone = 0;
    private const uint EddGetDeviceInterfaceName = 1;

    public static bool IsWinDiscMonitorDevice(string monitorDeviceName) =>
        string.Equals(monitorDeviceName, "WinDisc", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Fills <paramref name="dev"/> for a monitor device name from <see cref="WindowsMonitorApiImports.GetMonitorInfo"/>.
    /// Enumerates all monitor devices on the adapter and prefers the one with
    /// <see cref="DisplayDeviceStateFlags.AttachedToDesktop"/> (index 0 is often a stale Generic PnP entry).
    /// Tries <c>dwFlags=0</c> first (friendly <see cref="DISPLAY_DEVICE.DeviceString"/>); then
    /// <c>EDD_GET_DEVICE_INTERFACE_NAME</c> for paths that require it.
    /// </summary>
    public static bool TryGetDisplayDeviceForMonitor(string monitorDeviceName, out DISPLAY_DEVICE dev,
        out uint lastAttemptFlags, out int lastWin32Error)
    {
        lastAttemptFlags = EddFlagsNone;
        if (TrySelectMonitorDevice(monitorDeviceName, EddFlagsNone, out dev, out lastWin32Error))
            return true;

        lastAttemptFlags = EddGetDeviceInterfaceName;
        return TrySelectMonitorDevice(monitorDeviceName, EddGetDeviceInterfaceName, out dev, out lastWin32Error);
    }

    private static bool TrySelectMonitorDevice(string monitorDeviceName, uint dwFlags, out DISPLAY_DEVICE selected,
        out int lastWin32Error)
    {
        selected = default;
        var foundAny = false;
        var first = default(DISPLAY_DEVICE);

        for (uint iDevNum = 0; ; iDevNum++)
        {
            var candidate = new DISPLAY_DEVICE();
            candidate.cb = Marshal.SizeOf<DISPLAY_DEVICE>();
            if (!WindowsMonitorApiImports.EnumDisplayDevices(monitorDeviceName, iDevNum, ref candidate, dwFlags))
            {
                lastWin32Error = Marshal.GetLastWin32Error();
                break;
            }

            if (!foundAny)
            {
                first = candidate;
                foundAny = true;
            }

            if ((candidate.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) != 0)
            {
                selected = candidate;
                lastWin32Error = 0;
                return true;
            }
        }

        if (!foundAny)
            return false;

        selected = first;
        lastWin32Error = 0;
        return true;
    }
}
