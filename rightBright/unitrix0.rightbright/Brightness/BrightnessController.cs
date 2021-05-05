using System;
using System.Diagnostics;
using System.Linq;
using Prism.Mvvm;
using unitrix0.rightbright.Brightness.Calculators;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Sensors.Model;
using unitrix0.rightbright.Services.Brightness;
using unitrix0.rightbright.Settings;

namespace unitrix0.rightbright.Brightness
{
    public class BrightnessController : BindableBase, IBrightnessController
    {
        private readonly ISensorService _sensorService;
        private readonly ISetBrightnessService _brightnessService;
        private readonly IMonitorService _monitorService;
        private readonly IBrightnessCalculator _brightnessCalculator;
        private readonly ISettings _settings;
        private AmbientLightSensor _connectedSensor;

        public bool PauseSettingBrightness { get; set; }

        public AmbientLightSensor ConnectedSensor
        {
            get => _connectedSensor;
            private set => SetProperty(ref _connectedSensor, value);
        }

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
            _monitorService.UpdateList();
            LoadMonitorSettings();
            if (!TryConnectLastUsedSensor()) return;

            _sensorService.StartPollTimer();
        }

        public bool ConnectSensor(AmbientLightSensor sensor)
        {
            if (!_sensorService.ConnectToSensor(sensor.FriendlyName)) return false;

            ConnectedSensor = sensor;
            return true;
        }

        private bool TryConnectLastUsedSensor()
        {
            if (_settings.LastUsedSensor == null) return false;

            var sensors = _sensorService.GetSensors();
            var lastUsedSensor = sensors.FirstOrDefault(s => s.FriendlyName == _settings.LastUsedSensor.FriendlyName);
            if (lastUsedSensor == null) return false;

            lastUsedSensor.MaxValue = _settings.LastUsedSensor.MaxValue;
            lastUsedSensor.MinValue = _settings.LastUsedSensor.MinValue;

            return ConnectSensor(lastUsedSensor);
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
            Debug.Print($"****** Sensor Update: {e} lx ******");

            ConnectedSensor.CurrentValue = (int)Math.Round(e);
            if (PauseSettingBrightness) return;

            var monitors = _monitorService.Monitors.Where(m => m.CalculationParameters.Active);
            foreach (var monitor in monitors)
            {
                var newBrightness = (int)Math.Round(_brightnessCalculator.Calculate(e, monitor.CalculationParameters.Progression, monitor.CalculationParameters.Curve, monitor.CalculationParameters.MinBrightness));
                newBrightness = newBrightness > 100 ? 100 : newBrightness;

                if (newBrightness == monitor.LastBrightnessSet) continue;

                Debug.Print($"{DateTime.Now.TimeOfDay}\t Updating Brightness on {monitor.DeviceName} to: {newBrightness}");
                _brightnessService.SetBrightness(monitor.Handle, newBrightness);
                monitor.LastBrightnessSet = newBrightness;
            }
        }
    }
}