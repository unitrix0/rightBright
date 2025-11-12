using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using rightBright.Brightness.Calculators;
using rightBright.Models.Sensors;
using rightBright.Services.Brightness;
using rightBright.Services.Logging;
using rightBright.Services.Monitors;
using rightBright.Services.Sensors;
using rightBright.Services.SystemNotifications;
using rightBright.Services.SystemNotifications.Windows;
using rightBright.Settings;
using rightBright.WindowsApi.Monitor;
using Timer = System.Timers.Timer;

namespace rightBright.Brightness
{
    public class BrightnessController : ObservableObject, IBrightnessController
    {
        private readonly ISensorService _sensorService;
        private readonly ISetBrightnessService _brightnessService;
        private readonly IMonitorEnummerationService _monitorService;
        private readonly IBrightnessCalculator _brightnessCalculator;
        private readonly ISettings _settings;
        private readonly ILoggingService _logger;
        private AmbientLightSensor? _connectedSensor;
        private bool _pauseSettingBrightness;
        private readonly Timer _pollingRestartTimer;
        private bool _updatingStopped;

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
            IMonitorEnummerationService monitorService, IBrightnessCalculator brightnessCalculator, ISettings settings,
            IMonitorChangedNotificationService monitorNotificationService,
            IPowerNotificationService powerNotificationService, ILoggingService logger)
        {
            _sensorService = sensorService;
            _brightnessService = brightnessService;
            _monitorService = monitorService;
            _brightnessCalculator = brightnessCalculator;
            _settings = settings;
            _logger = logger;

            _pollingRestartTimer = new Timer() { Interval = 1500, AutoReset = false };
            _pollingRestartTimer.Elapsed += OnPollingRestartTimerElapsed;

            sensorService.Update += OnSensorUpdate;
            monitorNotificationService.DeviceChangedMessage += OnDeviceChangedMessage;
            powerNotificationService.ScreensPoweredOff += OnStopUpdating;
            powerNotificationService.SystemSuspending += OnStopUpdating;
            powerNotificationService.ScreensPoweredOn += OnRestartUpdating;
            powerNotificationService.SystemResuming += OnRestartUpdating;
        }

        /// <summary>
        /// Uses the previously used sensor
        /// </summary>
        public void Run()
        {
            _updatingStopped = true;
            LoadMonitorSettings();

            if (!ConnectSensor(_settings.LastUsedSensor)) return;

            _sensorService.StartPollTimer();
            _updatingStopped = false;
        }

        public bool Run(AmbientLightSensor sensor)
        {
            if (!ConnectSensor(sensor)) return false;
            _sensorService.StartPollTimer();

            return true;
        }

        private bool ConnectSensor(AmbientLightSensor sensor)
        {
            try
            {
                ConnectedSensor = _sensorService.GetSensors().SingleOrDefault(s => s.FriendlyName == sensor.FriendlyName);
                return ConnectedSensor != null && 
                       _sensorService.ConnectToSensor(ConnectedSensor.FriendlyName);
            }
            catch (Exception )
            {
                //TODO Log error message
                return false;
            }
        }

        private void LoadMonitorSettings()
        {
            _logger.WriteInformation("Loading monitor settings");
            
            foreach (var monitor in _monitorService.GetDisplays())
            {
                if (!_settings.BrightnessCalculationParameters.TryGetValue(monitor.ModelName, out var savedSettings))
                    continue;

                monitor.CalculationParameters.Progression = savedSettings.Progression;
                monitor.CalculationParameters.MinBrightness = savedSettings.MinBrightness;
                monitor.CalculationParameters.Curve = savedSettings.Curve;
                monitor.CalculationParameters.Active = savedSettings.Active;
            }
        }

        private void ResetPollingRestartTimer()
        {
            _logger.WriteInformation(nameof(ResetPollingRestartTimer));
            _pollingRestartTimer.Stop();
            _pollingRestartTimer.Start();
        }

        private void OnRestartUpdating(object? sender, EventArgs e)
        {
            if (!_updatingStopped) return;
            ResetPollingRestartTimer();
        }

        private void OnStopUpdating(object? sender, EventArgs e)
        {
            StopUpdating();
        }

        private void OnPollingRestartTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            _logger.WriteInformation("******** Trying  to restart polling ********");
            Dispatcher.UIThread.InvokeAsync(Run, DispatcherPriority.Normal);
        }

        private void OnDeviceChangedMessage(object? sender, EventArgs e)
        {
            _logger.WriteInformation(nameof(OnDeviceChangedMessage));
            LoadMonitorSettings();
        }

        private void StopUpdating()
        {
            if (_updatingStopped) return;
            _updatingStopped = true;
            _sensorService.StopPollTimer();
        }

        private async void OnSensorUpdate(object? sender, double e)
        {
            try
            {
                //Debug.Print($"****** Sensor Update: {e} lx ******");
                
                ConnectedSensor!.CurrentValue = (int)Math.Round(e);
                if (PauseSettingBrightness) return;

                var monitors = _monitorService.GetDisplays().Where(m => m.CalculationParameters.Active);
                foreach (var monitor in monitors)
                {
                    if (_updatingStopped) return;

                    var newBrightness = (int)Math.Round(_brightnessCalculator.Calculate(e,
                        monitor.CalculationParameters.Progression,
                        monitor.CalculationParameters.Curve, monitor.CalculationParameters.MinBrightness));
                    newBrightness = newBrightness > 100 ? 100 : newBrightness;

                    //Debug.Print($"{DateTime.Now.TimeOfDay}\t Updating Brightness on {monitor.DeviceName} to: {newBrightness}");
                    await _brightnessService.SetBrightness(monitor, newBrightness);
                    monitor.LastBrightnessSet = newBrightness;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError($"Error setting Brightness: {ex.Message}");
            }
        }
    }
}
