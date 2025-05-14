using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using rightBright.WindowsApi.Monitor;
using rightBright.WindowsApi.Monitor.Structs;
using unitrix0.rightbright.Monitors.Models;

namespace rightBright.Services.Monitors
{
    public class WinMonitorEnumService : IMonitorEnummerationService
    {
        /// <summary>
        /// Returns informations about the connect Mointors
        /// </summary>
        /// <returns>collection of Display Info</returns>
        public List<DisplayInfo> GetDisplays()
        {
            var col = new List<DisplayInfo>();

            bool ResultCallback(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData)
            {
                var mi = new MonitorInfoEx();
                mi.Size = Marshal.SizeOf(mi);

                var success = WindowsMonitorApiImports.GetMonitorInfo(hMonitor, ref mi);
                if (!success)
                {
                    //TODO Error Handling
                    var err = Marshal.GetLastWin32Error();
                    return false;
                }

                var dev = new DISPLAY_DEVICE();
                dev.cb = Marshal.SizeOf(dev);
                if (!WindowsMonitorApiImports.EnumDisplayDevices(mi.DeviceName, 0, ref dev, 1))
                {
                    //TODO Error Handling
                    var err = Marshal.GetLastWin32Error();
                }

                var di = new DisplayInfo
                {
                    ScreenWidth = mi.Monitor.Right - mi.Monitor.Left,
                    ScreenHeight = mi.Monitor.Bottom - mi.Monitor.Top,
                    MonitorArea = mi.Monitor,
                    WorkArea = mi.WorkArea,
                    IsPrimaryMonitor = Convert.ToBoolean(mi.Flags),
                    ModelName = dev.DeviceString,
                    DeviceName = mi.DeviceName
                };

                Debug.Print($"Display Found: {di.ModelName}");
                col.Add(di);
                return true;
            }

            WindowsMonitorApiImports.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, ResultCallback, IntPtr.Zero);

            return col;
        }

        public List<MonitorHandleInfo> GetMonitorHandles()
        {
            var col = new List<MonitorHandleInfo>();

            bool ResultCallback(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData)
            {
                var mi = new MonitorInfoEx();
                mi.Size = Marshal.SizeOf(mi);

                var success = WindowsMonitorApiImports.GetMonitorInfo(hMonitor, ref mi);
                if (!success)
                {
                    //TODO Error Handling
                    var err = Marshal.GetLastWin32Error();
                    return false;
                }

                var dev = new DISPLAY_DEVICE();
                dev.cb = Marshal.SizeOf(dev);
                if (!WindowsMonitorApiImports.EnumDisplayDevices(mi.DeviceName, 0, ref dev, 1))
                {
                    //TODO Error Handling
                    var err = Marshal.GetLastWin32Error();
                }

                var di = new MonitorHandleInfo(mi.DeviceName, hMonitor);
                col.Add(di);
                return true;
            }

            WindowsMonitorApiImports.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, ResultCallback, IntPtr.Zero);

            return col;
        }
    }
}
