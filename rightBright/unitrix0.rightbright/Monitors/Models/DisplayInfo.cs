using System;
using unitrix0.rightbright.Services.MonitorAPI.Structs;

namespace unitrix0.rightbright.Monitors.Models
{
    /// <summary>
    /// The struct that contains the display information
    /// </summary>
    public class DisplayInfo
    {
        public bool IsPrimaryMonitor { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }

        public string Resolution => $"{ScreenWidth}x{ScreenHeight}";
        public RectStruct MonitorArea { get; set; }
        public RectStruct WorkArea { get; set; }
        public string DeviceName { get; set; }

        public IntPtr Handle { get; set; }
        public string DeviceId { get; set; }
        public bool Active { get; set; }

        public override string ToString()
        {
            return $"{DeviceName} {Resolution}";
        }
    }
}