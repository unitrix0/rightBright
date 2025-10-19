using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Services.MonitorAPI.Structs;

namespace unitrix0.rightbright.Services.MonitorAPI
{
    public class MonitorEnummerationService : IMonitorEnummerationService
    {
        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum,
            IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice,
            uint dwFlags);

        private delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor,
            IntPtr dwData);

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

                var success = GetMonitorInfo(hMonitor, ref mi);
                if (!success)
                {
                    //TODO Error Handling
                    var err = Marshal.GetLastWin32Error();
                    return false;
                }

                var dev = new DISPLAY_DEVICE();
                dev.cb = Marshal.SizeOf(dev);
                if (!EnumDisplayDevices(mi.DeviceName, 0, ref dev, 1))
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
                    DeviceName = dev.DeviceID
                };

                Debug.Print($"Display Found: {di.ModelName}");
                col.Add(di);
                return true;
            }

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, ResultCallback, IntPtr.Zero);

            return col;
        }

        public List<MonitorHandleInfo> GetMonitorHandles()
        {
            var col = new List<MonitorHandleInfo>();

            bool ResultCallback(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData)
            {
                var mi = new MonitorInfoEx();
                mi.Size = Marshal.SizeOf(mi);

                var success = GetMonitorInfo(hMonitor, ref mi);
                if (!success)
                {
                    //TODO Error Handling
                    var err = Marshal.GetLastWin32Error();
                    return false;
                }

                var dev = new DISPLAY_DEVICE();
                dev.cb = Marshal.SizeOf(dev);
                if (!EnumDisplayDevices(mi.DeviceName, 0, ref dev, 1))
                {
                    //TODO Error Handling
                    var err = Marshal.GetLastWin32Error();
                }

                var di = new MonitorHandleInfo(dev.DeviceID, hMonitor);
                col.Add(di);
                return true;
            }

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, ResultCallback, IntPtr.Zero);

            return col;
        }
    }
}
