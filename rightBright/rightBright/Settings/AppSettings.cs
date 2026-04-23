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

        /// <summary>
        /// Used to detect the app's first run so we can sync initial auto-start state
        /// with platform defaults (installer behavior) before the user has configured it.
        /// </summary>
        [JsonIgnore] public bool SettingsFileExisted { get; private set; }

        public string HubUrl { get; set; } = "USB";
        public int YapiEventsTimerInterval { get; set; } = 5000;
        public AmbientLightSensor LastUsedSensor { get; set; } = new();
        public DateTime? LastUpdateCheckUtc { get; set; }
        public int UpdateCheckIntervalHours { get; set; } = 6;

        public Dictionary<string, BrightnessCalculationParameters> BrightnessCalculationParameters { get; set; } =
            new();

        public bool AutostartEnabled { get; set; }
        public int DeviceChangeDebounceMilliseconds { get; set; } = 5000;


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
                if (!File.Exists(sourceFileName)) return;
                
                File.Copy(sourceFileName, destFileName, true);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error while creating settings file backup. See See {nameof(Exception.InnerException)} for details",
                    ex);
            }
        }

        public static AppSettings Load()
        {
            var jsonFile = Path.Combine(SettingsFolder, "settings.json");
            if (!File.Exists(jsonFile))
            {
                return new AppSettings { SettingsFileExisted = false };
            }

            var settings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(jsonFile)) ?? new AppSettings();
            settings.SettingsFileExisted = true;
            return settings;
        }
    }
}
