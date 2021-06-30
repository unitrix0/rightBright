using System.Collections.Generic;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Settings
{
    public interface ISettings
    {
        string HubUrl { get; set; }
        int YapiEventsTimerInterval { get; set; }
        AmbientLightSensor? LastUsedSensor { get; set; }
        Dictionary<string, BrightnessCalculationParameters> BrightnessCalculationParameters { get; set; }
        void Save();
    }
}