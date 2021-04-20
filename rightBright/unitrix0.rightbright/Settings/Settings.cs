using System;
using System.IO;
using System.Net.Mime;
using System.Windows;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Settings
{
    public class Settings : ISettings
    {
        public string HubUrl { get; set; } = "USB";
        public int YapiEventsTimerInterval { get; set; } = 5000;
        public AmbientLightSensor LastUsedSensor { get; set; }


        public Settings()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory($"{appData}\\rightBright");
        }
        public void Save()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            File.WriteAllText($"{appData}\\rightBright\\settings.json", JsonConvert.SerializeObject(this));
        }

        public static ISettings Load()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!File.Exists($"{appData}\\rightBright\\settings.json")) return new Settings();

            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText($"{appData}\\rightBright\\settings.json"));
        }
    }
}