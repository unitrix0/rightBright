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
using rightBright.Services.LoadingState;
using Serilog;
using rightBright.Services.Monitors;
using rightBright.Services.Sensors;
using rightBright.Services.SystemNotifications;
using rightBright.Settings;
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
        private readonly ILogger _logger;
        private readonly ILoadingMonitorStateService _loadingMonitorStateService;
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

        public BrightnessController(ISensorService sensorService,
            ISetBrightnessService brightnessService,
            IMonitorEnummerationService monitorService,
            IBrightnessCalculator brightnessCalculator,
            ISettings settings,
            IMonitorChangedNotificationService monitorNotificationService,
            IPowerNotificationService powerNotificationService,
            ILogger logger,
            ILoadingMonitorStateService loadingMonitorStateService)
        {
            _sensorService = sensorService;
            _brightnessService = brightnessService;
            _monitorService = monitorService;
            _brightnessCalculator = brightnessCalculator;
            _settings = settings;
            _logger = logger;
            _loadingMonitorStateService = loadingMonitorStateService;

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
        /// Uses the previously used sensor from settings
        /// </summary>
        public async Task Run()
        {
            _updatingStopped = true;
            await LoadMonitorSettings();

            // Get the cached sensor instance from the sensor service instead of using the deserialized settings object
            var sensorToConnect = _sensorService.GetSensors()
                .SingleOrDefault(s => s.SerialNumber == _settings.LastUsedSensor?.SerialNumber);

            if (sensorToConnect == null || !ConnectSensor(sensorToConnect)) return;

            _updatingStopped = false;
            _sensorService.StartPollTimer();
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
                // ConnectedSensor = _sensorService.GetSensors().SingleOrDefault(s => s.FriendlyName == sensor.FriendlyName);
                // if (ConnectedSensor == null) return false;
                
                if (!_sensorService.ConnectToSensor(sensor.FriendlyName)) return false;
                
                ConnectedSensor = sensor;
                _logger.Information($"Sensor {sensor.FriendlyName} connected");
                return true;

            }
            catch (Exception ex)
            {
                _logger.Error($"Connection to '{ConnectedSensor?.FriendlyName}' failed: {ex.Message}");
                return false;
            }
        }

        private async Task LoadMonitorSettings()
        {
            _logger.Information("Loading monitor settings");

            try
            {
                _loadingMonitorStateService.IsLoading = true;

                foreach (var monitor in await _monitorService.GetDisplays())
                {
                    if (!_settings.BrightnessCalculationParameters.TryGetValue(monitor.ModelName, out var savedSettings))
                        continue;

                    monitor.CalculationParameters.MinBrightness = savedSettings.MinBrightness;
                    monitor.CalculationParameters.ControlPointX = savedSettings.ControlPointX;
                    monitor.CalculationParameters.ControlPointY = savedSettings.ControlPointY;
                    monitor.CalculationParameters.MaxLux = savedSettings.MaxLux;
                    monitor.CalculationParameters.Active = savedSettings.Active;
                }
            }
            finally
            {
                _loadingMonitorStateService.IsLoading = false;
            }
        }

        private void ResetPollingRestartTimer()
        {
            _logger.Information(nameof(ResetPollingRestartTimer));
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
            _logger.Information("******** Trying  to restart polling ********");
            Dispatcher.UIThread.InvokeAsync(Run, DispatcherPriority.Normal);
        }

        private void OnDeviceChangedMessage(object? sender, EventArgs e)
        {
            _logger.Information(nameof(OnDeviceChangedMessage));
            _ = LoadMonitorSettings();
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
                _logger.Debug($"[SensorUpdate] Lux: {e:F1}");

                ConnectedSensor!.CurrentValue = (int)Math.Round(e);
                if (PauseSettingBrightness)
                {
                    _logger.Debug("[SensorUpdate] Paused — skipping brightness update");
                    return;
                }

                List<DisplayInfo> activeDisplays;
                try
                {
                    _loadingMonitorStateService.IsLoading = true;

                    activeDisplays = (await _monitorService.GetDisplays())
                        .Where(m => m.CalculationParameters.Active)
                        .ToList();
                }
                finally
                {
                    _loadingMonitorStateService.IsLoading = false;
                }

                _logger.Debug($"[SensorUpdate] Active displays: {activeDisplays.Count}");

                foreach (var display in activeDisplays)
                {
                    if (_updatingStopped)
                    {
                        _logger.Warning("[SensorUpdate] Updating stopped — aborting loop");
                        return;
                    }

                    var p = display.CalculationParameters;
                    var rawBrightness = _brightnessCalculator.Calculate(e, p);
                    var newBrightness = (int)Math.Round(rawBrightness);
                    newBrightness = Math.Min(newBrightness, 100);

                    _logger.Debug(
                        $"[SensorUpdate] '{display.ModelName}': params(min={p.MinBrightness}, cpX={p.ControlPointX:F1}, cpY={p.ControlPointY:F1}, maxLux={p.MaxLux}) -> raw={rawBrightness:F2}, rounded={newBrightness}%");

                    bool successful;
                    var trycount = 0;
                    do
                    {
                        if (trycount > 0)
                        {
                            _logger.Warning($"[SensorUpdate] '{display.ModelName}': retry {trycount + 1} for brightness {newBrightness}");
                            Task.Delay(250).Wait();
                        }
                        successful = await _brightnessService.SetBrightness(display, newBrightness);
                        trycount++;
                    } while (!successful && trycount < 4);

                    if (successful)
                    {
                        _logger.Debug(
                            $"[SensorUpdate] '{display.ModelName}': SetBrightness succeeded, updating LastBrightnessSet {display.LastBrightnessSet} -> {newBrightness}");
                        display.LastBrightnessSet = newBrightness;
                    }
                    else
                    {
                        _logger.Error(
                            $"[SensorUpdate] '{display.ModelName}': SetBrightness failed after {trycount} attempts, LastBrightnessSet remains {display.LastBrightnessSet}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[SensorUpdate] Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
