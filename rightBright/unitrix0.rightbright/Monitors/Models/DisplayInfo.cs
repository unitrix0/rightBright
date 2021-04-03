using System;
using SetBrightness;

namespace unitrix0.rightbright.Monitors.Models
{
    /// <summary>
    /// The struct that contains the display information
    /// </summary>
    public class DisplayInfo
    {
        public bool IsPrimaryMonitor { get; set; }
        public string ScreenHeight { get; set; }
        public string ScreenWidth { get; set; }
        public RectStruct MonitorArea { get; set; }
        public RectStruct WorkArea { get; set; }
        public string DeviceName { get; set; }

        public IntPtr Handle { get; set; }

        public override string ToString()
        {
            return $"{ScreenWidth} * {ScreenHeight}";
        }
    }
}