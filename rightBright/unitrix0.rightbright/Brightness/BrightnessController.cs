using System;
using System.Diagnostics;
using System.Linq;
using unitrix0.rightbright.Brightness.Calculators;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Sensors.Model;
using unitrix0.rightbright.Services.Brightness;
using unitrix0.rightbright.Settings;

namespace unitrix0.rightbright.Brightness
{
    public class BrightnessController : IBrightnessController
    {
        private readonly ISensorService _sensorService;
        private readonly ISetBrightnessService _brightnessService;
        private readonly IMonitorService _monitorService;
        private readonly IBrightnessCalculator _brightnessCalculator;
        private readonly ISettings _settings;

        public BrightnessController(ISensorService sensorService, ISetBrightnessService brightnessService, 
            IMonitorService monitorService, IBrightnessCalculator brightnessCalculator, ISettings settings)
        {
            _sensorService = sensorService;
            _brightnessService = brightnessService;
            _monitorService = monitorService;
            _brightnessCalculator = brightnessCalculator;
            _settings = settings;

            sensorService.Update += OnSensorUpdate;
        }

        public void Run()
        {
            if (!TryConnectLastUsedSensor()) return;
            _monitorService.UpdateList();
            LoadMonitorSettings();

            _sensorService.StartPollTimer();
        }

        private bool TryConnectLastUsedSensor()
        {
            if (_settings.LastUsedSensor == null) return false;

            var sensors = _sensorService.GetSensors();
            var lastUsedSensor = sensors.FirstOrDefault(s => s.FriendlyName == _settings.LastUsedSensor.FriendlyName);
            if (lastUsedSensor == null) return false;
            
            return _sensorService.ConnectToSensor(lastUsedSensor.FriendlyName);
        }

        private void LoadMonitorSettings()
        {
            foreach (var monitor in _monitorService.Monitors)
            {
                if (!_settings.BrightnessCalculationParameters.ContainsKey(monitor.DeviceName)) continue;
                
                var savedSettings = _settings.BrightnessCalculationParameters[monitor.DeviceName];
                monitor.CalculationParameters.Progression = savedSettings.Progression;
                monitor.CalculationParameters.MinBrightness = savedSettings.MinBrightness;
                monitor.CalculationParameters.Curve = savedSettings.Curve;
                monitor.CalculationParameters.Active = savedSettings.Active;
            }
        }

        private void OnSensorUpdate(object sender, double e)
        {
            Debug.Print($"****** Sensor Update: {e} lux ******");
            var monitors = _monitorService.Monitors.Where(m => m.CalculationParameters.Active);

            foreach (var monitor in monitors)
            {
                var newBrightness = (int)Math.Round(_brightnessCalculator.Calculate(e, monitor.CalculationParameters.Progression, monitor.CalculationParameters.Curve, monitor.CalculationParameters.MinBrightness));
                if (newBrightness == monitor.LastBrightnessSet) continue;

                Debug.Print($"{DateTime.Now.TimeOfDay}\t Updating Brightness on {monitor.DeviceName} to: {newBrightness}");
                _brightnessService.SetBrightness(monitor.Handle, newBrightness);
                monitor.LastBrightnessSet = newBrightness;
            }
        }
    }
}