using System;
using Microsoft.VisualBasic;
using Prism.Mvvm;
using unitrix0.rightbright.Services.MonitorAPI.Structs;

namespace unitrix0.rightbright.Monitors.Models
{
    public class DisplayInfo : BindableBase
    {
        private bool _active;
        private int _lastBrightnessSet;
        public bool IsPrimaryMonitor { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }

        public string Resolution => $"{ScreenWidth}x{ScreenHeight}";
        public RectStruct MonitorArea { get; set; }
        public RectStruct WorkArea { get; set; }
        public string DeviceName { get; set; }

        public IntPtr Handle { get; set; }
        public string DeviceId { get; set; }

        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }

        public int LastBrightnessSet
        {
            get => _lastBrightnessSet;
            set => SetProperty(ref _lastBrightnessSet, value);
        }

        public BrightnessCalculationParameters CalculationParameters { get; set; }

        public DisplayInfo()
        {
            CalculationParameters = new BrightnessCalculationParameters();
            _lastBrightnessSet = -1;
        }

        public override string ToString()
        {
            return $"{DeviceName} {Resolution}";
        }
    }
}