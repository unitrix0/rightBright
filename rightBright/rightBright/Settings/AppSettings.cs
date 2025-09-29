using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using rightBright.Models.Monitors;
using rightBright.Models.Sensors;

namespace rightBright.Settings
{
    public class AppSettings : ISettings
    {
        private static readonly string SettingsFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "rightBright");

        public string HubUrl { get; set; } = "USB";
        public int YapiEventsTimerInterval { get; set; } = 5000;
        public AmbientLightSensor LastUsedSensor { get; set; } = new();

        public Dictionary<string, BrightnessCalculationParameters> BrightnessCalculationParameters { get; set; } =
            new();


        public void Save()
        {
            if (!Directory.Exists(SettingsFolder)) Directory.CreateDirectory(SettingsFolder);

            var settingsJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (string.IsNullOrEmpty(settingsJson)) return;

            CeateBackup();
            var path = Path.Combine(SettingsFolder, "settings.json");
            File.WriteAllText(path, settingsJson);
        }

        private void CeateBackup()
        {
            try
            {
                var backupFilename = "settings.bkp";
                var sourceFileName = Path.Combine(SettingsFolder, "settings.json");
                var destFileName = Path.Combine(SettingsFolder, backupFilename);
                
                File.Copy(sourceFileName, destFileName, true);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error while creating settings file backup. See See {nameof(Exception.InnerException)} for details",
                    ex);
            }
        }

        public static ISettings Load()
        {
            var jsonFile = Path.Combine(SettingsFolder, "settings.json");
            if (!File.Exists(jsonFile)) return new AppSettings();

            return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(jsonFile)) ?? new AppSettings();
        }
    }
}
