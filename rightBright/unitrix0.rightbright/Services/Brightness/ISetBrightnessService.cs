using System;

namespace unitrix0.rightbright.Services.Brightness
{
    public interface ISetBrightnessService
    {
        uint CurrentBrightness { get; }
        void SetBrightness(IntPtr monitorHandle, int newValue);
    }
}