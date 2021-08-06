using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Settings
{
    public class Settings : ISettings
    {
        private static readonly string SettingsFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\rightBright";
        public string HubUrl { get; set; } = "USB";
        public int YapiEventsTimerInterval { get; set; } = 5000;
        public AmbientLightSensor LastUsedSensor { get; set; }

        public Dictionary<string, BrightnessCalculationParameters> BrightnessCalculationParameters { get; set; }


        public Settings()
        {
            LastUsedSensor = new AmbientLightSensor();
            BrightnessCalculationParameters = new Dictionary<string, BrightnessCalculationParameters>();

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory($"{appData}");
        }
        public void Save()
        {
            if (!Directory.Exists(SettingsFolder)) Directory.CreateDirectory(SettingsFolder);

            var settingsJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (string.IsNullOrEmpty(settingsJson)) return;

            CeateBackup();
            File.WriteAllText($"{SettingsFolder}\\settings.json", settingsJson);
        }

        private void CeateBackup()
        {
            try
            {
                string backupFilename = EvalBackupFileName();
                File.Copy($"{SettingsFolder}\\settings.json", $"{SettingsFolder}\\{backupFilename}");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error while creating settings file backup. See See {nameof(Exception.InnerException)} for details",
                    ex);
            }
        }

        private string EvalBackupFileName(bool secondRun = false)
        {
            var i = 0;
            while (File.Exists($"{SettingsFolder}\\settings_{i}.json") && i < 5)
            {
                i++;
            }

            if (i != 6) return $"settings_{i}.json";
            if (secondRun) throw new Exception("Failed to evaluate backup file name");

            DeleteOldestSettingsFile();
            return EvalBackupFileName(true);
        }

        private void DeleteOldestSettingsFile()
        {
            try
            {
                var settingsDir = new DirectoryInfo(SettingsFolder);
                var files = settingsDir.EnumerateFiles();

                var oldestFile = files
                    .Where(fi => fi.Name != "settings.json")
                    .OrderByDescending(fi => fi.CreationTime)
                    .FirstOrDefault();

                if (oldestFile == null) throw new Exception($"Oldest settings file could not be evaluated");
                oldestFile.Delete();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error deleting oldest settings file. See {nameof(Exception.InnerException)} for details", ex);
            }
        }

        public static ISettings Load()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var jsonFile = $"{appData}\\rightBright\\settings.json";
            if (!File.Exists(jsonFile)) return new Settings();

            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(jsonFile)) ?? new Settings();
        }
    }
}