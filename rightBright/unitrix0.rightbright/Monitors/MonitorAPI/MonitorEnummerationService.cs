using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SetBrightness;
using unitrix0.rightbright.Monitors.Models;

namespace unitrix0.rightbright.Monitors.MonitorAPI
{
    public class MonitorEnummerationService
    {
        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        private delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData);


        /// <summary>
        /// Returns informations about the connect Mointors
        /// </summary>
        /// <returns>collection of Display Info</returns>
        public List<DisplayInfo> GetDisplays()
        {
            var col = new List<DisplayInfo>();

            bool Result(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData)
            {
                var mi = new MonitorInfoEx();
                mi.Size = Marshal.SizeOf(mi);
                var success = GetMonitorInfo(hMonitor, ref mi);
                if (!success) return false;

                var di = new DisplayInfo
                {
                    ScreenWidth = (mi.Monitor.Right - mi.Monitor.Left).ToString(),
                    ScreenHeight = (mi.Monitor.Bottom - mi.Monitor.Top).ToString(),
                    MonitorArea = mi.Monitor,
                    WorkArea = mi.WorkArea,
                    IsPrimaryMonitor = Convert.ToBoolean(mi.Flags),
                    DeviceName = mi.DeviceName,
                    Handle = hMonitor
                };
                col.Add(di);
                return true;
            }

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, Result, IntPtr.Zero);

            return col;
        }
    }
}