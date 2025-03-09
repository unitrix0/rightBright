using System.Collections.Generic;
using rightBright.Models.Monitors;
using rightBright.Sensors.Model;

namespace rightBright.Settings
{
    public interface ISettings
    {
        string HubUrl { get; set; }
        int YapiEventsTimerInterval { get; set; }
        AmbientLightSensor LastUsedSensor { get; set; }
        Dictionary<string, BrightnessCalculationParameters> BrightnessCalculationParameters { get; set; }
        void Save();
    }
}
