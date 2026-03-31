using System;

namespace unitrix0.rightbright.Monitors.Models;

public class MonitorHandleInfo
{
    public string DeviceName { get; private set; }
    public IntPtr Handle { get; private set; }

    public MonitorHandleInfo(string deviceName, IntPtr handle)
    {
        DeviceName = deviceName;
        Handle = handle;
    }
}