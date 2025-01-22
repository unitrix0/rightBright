using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using unitrix0.rightbright.Brightness.Calculators;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Sensors.Model;
using unitrix0.rightbright.Services.Brightness;
using unitrix0.rightbright.Services.Logging;
using unitrix0.rightbright.Services.MonitorAPI;
using unitrix0.rightbright.Services.TrayIcon;
using unitrix0.rightbright.Settings;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;
using Timer = System.Timers.Timer;

namespace unitrix0.rightbright.Brightness
{
    public class BrightnessController : BindableBase, IBrightnessController
    {
        private readonly ISensorService _sensorService;
        private readonly ISetBrightnessService _brightnessService;
        private readonly IMonitorService _monitorService;
        private readonly IBrightnessCalculator _brightnessCalculator;
        private readonly ISettings _settings;
        private readonly ITrayIcon _trayIcon;
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
            IMonitorService monitorService, IBrightnessCalculator brightnessCalculator, ISettings settings,
            IDeviceChangedNotificationService deviceNotificationService,
            IPowerNotificationService powerNotificationService,
            ITrayIcon trayIcon, ILoggingService logger)
        {
            _sensorService = sensorService;
            _brightnessService = brightnessService;
            _monitorService = monitorService;
            _brightnessCalculator = brightnessCalculator;
            _settings = settings;
            _trayIcon = trayIcon;
            _logger = logger;

            _pollingRestartTimer = new Timer() { Interval = 1500, AutoReset = false };
            _pollingRestartTimer.Elapsed += OnPollingRestartTimerElapsed;

            sensorService.Update += OnSensorUpdate;
            deviceNotificationService.DeviceChangedMessage += OnDeviceChangedMessage;
            deviceNotificationService.UsbDeviceConnectedMessage += OnUsbDeviceConnectedMessage;
            deviceNotificationService.UsbDeviceDisconnectedMessage += OnUsbDeviceDisconnectMessage;
            powerNotificationService.ScreensPoweredOff += OnStopUpdating;
            powerNotificationService.SystemSuspending += OnStopUpdating;
            powerNotificationService.ScreensPoweredOn += OnRestartUpdating;
            powerNotificationService.SystemResuming += OnRestartUpdating;
        }

        public void Run()
        {
            _updatingStopped = true;
            _monitorService.UpdateList();
            LoadMonitorSettings();
            if (!TryConnectLastUsedSensor()) return;

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
            _logger.WriteInformation($"Connecting sensor... (isOnline: {sensor.IsOnline}, isReady: {sensor.IsReady})");
            if (!_sensorService.ConnectToSensor(sensor.FriendlyName)) return false;

            _logger.WriteInformation("Successful!");
            ConnectedSensor = sensor;
            return true;
        }

        private bool TryConnectLastUsedSensor()
        {
            _logger.WriteInformation($"{nameof(TryConnectLastUsedSensor)}");

            AmbientLightSensor? lastUsedSensor = null;
            int tries = 0;
            while (lastUsedSensor == null && tries <= 2)
            {
                _logger.WriteInformation($"\tTry {tries + 1}");
                var sensors = _sensorService.GetSensors(true);
                lastUsedSensor = sensors.FirstOrDefault(s => s.FriendlyName == _settings.LastUsedSensor.FriendlyName);
                tries++;
                Task.Delay(1000);
            }

            if (lastUsedSensor == null)
            {
                _logger.WriteWarning("Last used sensor not found! No polling started!");
                return false;
            }

            if (!lastUsedSensor.IsOnline || !lastUsedSensor.IsReady) return false;

            lastUsedSensor.MaxValue = _settings.LastUsedSensor.MaxValue;
            lastUsedSensor.MinValue = _settings.LastUsedSensor.MinValue;

            return ConnectSensor(lastUsedSensor);
        }

        private void LoadMonitorSettings()
        {
            _logger.WriteInformation("Loading monitor settings");
            foreach (var monitor in _monitorService.Monitors)
            {
                if (!_settings.BrightnessCalculationParameters.ContainsKey(monitor.ModelName)) continue;

                var savedSettings = _settings.BrightnessCalculationParameters[monitor.ModelName];
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
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(Run));
        }

        private void OnDeviceChangedMessage(object? sender, EventArgs e)
        {
            _logger.WriteInformation(nameof(OnDeviceChangedMessage));
            StopUpdating();
            ResetPollingRestartTimer();
        }

        private void OnUsbDeviceDisconnectMessage(object? sender, EventArgs e)
        {
            _logger.WriteInformation(nameof(OnUsbDeviceDisconnectMessage));
            if (_sensorService.Connected) return;
            _logger.WriteWarning("Ambient light sensor disconnected!");
            _trayIcon.ShowWarningBallon("rightBright", "Ambient light sensor disconnected!");
            StopUpdating();
        }

        private void OnUsbDeviceConnectedMessage(object? sender, EventArgs e)
        {
            _logger.WriteInformation(nameof(OnUsbDeviceConnectedMessage));

            if (!_updatingStopped) return;
            ResetPollingRestartTimer();
        }

        private void StopUpdating()
        {
            if (_updatingStopped) return;
            _updatingStopped = true;
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
                if (_updatingStopped) return;

                var newBrightness = (int)Math.Round(_brightnessCalculator.Calculate(e,
                    monitor.CalculationParameters.Progression,
                    monitor.CalculationParameters.Curve, monitor.CalculationParameters.MinBrightness));
                newBrightness = newBrightness > 100 ? 100 : newBrightness;

                //Debug.Print($"{DateTime.Now.TimeOfDay}\t Updating Brightness on {monitor.DeviceName} to: {newBrightness}");
                _brightnessService.SetBrightness(monitor, newBrightness);
                monitor.LastBrightnessSet = newBrightness;
            }
        }
    }
}
