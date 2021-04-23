using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Settings
{
    public class Settings : ISettings
    {
        public string HubUrl { get; set; } = "USB";
        public int YapiEventsTimerInterval { get; set; } = 5000;
        public AmbientLightSensor LastUsedSensor { get; set; }

        public Dictionary<string,BrightnessCalculationParameters> BrightnessCalculationParameters { get; set; }


        public Settings()
        {
            BrightnessCalculationParameters = new Dictionary<string, BrightnessCalculationParameters>();

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory($"{appData}\\rightBright");
        }
        public void Save()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            File.WriteAllText($"{appData}\\rightBright\\settings.json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static ISettings Load()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!File.Exists($"{appData}\\rightBright\\settings.json")) return new Settings();

            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText($"{appData}\\rightBright\\settings.json"));
        }
    }
}