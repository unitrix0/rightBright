using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Prism.Mvvm;
using unitrix0.rightbright.Brightness.Calculators;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Sensors.Model;
using unitrix0.rightbright.Services.Brightness;
using unitrix0.rightbright.Services.MonitorAPI;
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
        private AmbientLightSensor? _connectedSensor;
        private bool _pauseSettingBrightness;
        private readonly Timer _restartTimer;
        private bool _stopUpdating;

        public bool PauseSettingBrightness
        {
            get => _pauseSettingBrightness;
            set
            {
                Debug.Print($"\t\t{nameof(PauseSettingBrightness)} = {value}");
                _pauseSettingBrightness = value;
            }
        }

        public AmbientLightSensor? ConnectedSensor
        {
            get => _connectedSensor;
            private set => SetProperty(ref _connectedSensor, value);
        }

        public BrightnessController(ISensorService sensorService, ISetBrightnessService brightnessService,
            IMonitorService monitorService, IBrightnessCalculator brightnessCalculator, ISettings settings,
            IDeviceChangedNotificationService deviceNotificationService, IPowerNotificationService powerNotificationService)
        {
            _sensorService = sensorService;
            _brightnessService = brightnessService;
            _monitorService = monitorService;
            _brightnessCalculator = brightnessCalculator;
            _settings = settings;

            _restartTimer = new Timer() { Interval = 3000, AutoReset = false };
            _restartTimer.Elapsed += OnRestartTimerElapsed;

            sensorService.Update += OnSensorUpdate;
            deviceNotificationService.DeviceChangedMessage += OnDeviceChangedMessage;
            powerNotificationService.ScreensPoweredOff += OnStopUpdating;
            powerNotificationService.SystemSuspending += OnStopUpdating;
            powerNotificationService.ScreensPoweredOn += OnRestartUpdating;
            powerNotificationService.SystemResuming += OnRestartUpdating;
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

            Debug.Print($"\t\t{nameof(TryConnectLastUsedSensor)}");
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

        private void ResetRestartTimer()
        {
            Debug.WriteLine($"******** {nameof(ResetRestartTimer)} ********");
            _restartTimer.Stop();
            _restartTimer.Start();
        }

        private void OnRestartUpdating(object? sender, EventArgs e)
        {
            // Event is also fired on App startup because of a
            // "Power On" Window Message, but no reason to reset the timer
            if(!_stopUpdating) return;
            ResetRestartTimer();
        }

        private void OnStopUpdating(object? sender, EventArgs e)
        {
            StopUpdating();
        }

        private void OnRestartTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Debug.Print("******** Restarting ********");
            _stopUpdating = false;
            Run();
        }

        private void OnDeviceChangedMessage(object? sender, EventArgs e)
        {
            StopUpdating();
            ResetRestartTimer();
            //if (_monitorUpdateInProgress) return;

            //Debug.Print($"\t\t{nameof(OnDeviceChangedMessage)}");
            //_monitorUpdateInProgress = true;

            //var lastPauseSetBrightnessValue = PauseSettingBrightness;
            //PauseSettingBrightness = true;

            //_monitorService.UpdateList();
            //LoadMonitorSettings();
            //PauseSettingBrightness = lastPauseSetBrightnessValue;
            //_monitorUpdateInProgress = false;
        }

        private void StopUpdating()
        {
            _stopUpdating = true;
            _sensorService.StopPollTimer();
        }

        private void OnSensorUpdate(object? sender, double e)
        {
            //Debug.Print($"****** Sensor Update: {e} lx ******");

            ConnectedSensor!.CurrentValue = (int)Math.Round(e);
            if (PauseSettingBrightness) return;

            var monitors = _monitorService.Monitors.Where(m => m.CalculationParameters.Active);
            foreach (var monitor in monitors)
            {
                if(_stopUpdating) return;

                var newBrightness = (int)Math.Round(_brightnessCalculator.Calculate(e, monitor.CalculationParameters.Progression, monitor.CalculationParameters.Curve, monitor.CalculationParameters.MinBrightness));
                newBrightness = newBrightness > 100 ? 100 : newBrightness;

                if (newBrightness == monitor.LastBrightnessSet) continue;

                //Debug.Print($"{DateTime.Now.TimeOfDay}\t Updating Brightness on {monitor.DeviceName} to: {newBrightness}");
                _brightnessService.SetBrightness(monitor.Handle, newBrightness);
                monitor.LastBrightnessSet = newBrightness;
            }
        }
    }
}