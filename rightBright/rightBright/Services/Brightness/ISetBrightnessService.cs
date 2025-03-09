using System;
using unitrix0.rightbright.Monitors.Models;

namespace unitrix0.rightbright.Services.Brightness
{
    public interface ISetBrightnessService
    {
        void SetBrightness(DisplayInfo monitor, int newValue);
    }
}
