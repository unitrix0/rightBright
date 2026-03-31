using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using rightBright.Models.Sensors;
using Serilog;
using rightBright.Settings;

namespace rightBright.Services.Sensors
{
    public class YoctoSensorService : ISensorService
    {
        private readonly ILogger _logger;
        private readonly Timer _handleYapiEventsTimer = new();
        private readonly ISensorRepo _sensorRepo;
        private YLightSensor? _sensorDevice;
        private string _error = "";
        private bool _sensorInitialized;
        private readonly object _historyLock = new();

        public event EventHandler<double>? Update;

        public string Error => _error;
        public string FriendlyName => _sensorDevice?.FriendlyName ?? "";
        public string Unit => _sensorDevice?.get_unit() ?? "";
        public bool SensorDeviceOnline => _sensorDevice?.isOnline() ?? false;

        public Queue<LuxReading> ValueHistory { get; }

        public YoctoSensorService(ISensorRepo sensorRepo, ISettings settings, ILogger logger)
        {
            _sensorRepo = sensorRepo;
            _logger = logger;
            _handleYapiEventsTimer.Elapsed += HandleYapiEventsTimerOnElapsed;
            _handleYapiEventsTimer.Interval = settings.YapiEventsTimerInterval;
            ValueHistory = new Queue<LuxReading>(17280);

            YAPI.RegisterHub(settings.HubUrl, ref _error);
        }


        public bool ConnectToSensor(string sensorFriendlyName)
        {
            _sensorDevice = YLightSensor.FindLightSensor(sensorFriendlyName);
            _sensorDevice.registerTimedReportCallback(TimedReport);
            lock (_historyLock) { ValueHistory.Clear(); }
            return true;
        }

        public LuxReading[] GetValueHistorySnapshot()
        {
            lock (_historyLock) { return ValueHistory.ToArray(); }
        }

        public List<AmbientLightSensor> GetSensors(bool forceUpdate = false)
        {
            return _sensorRepo.GetSensors(forceUpdate);
        }

        public void StartPollTimer()
        {
            if (_sensorDevice == null) throw new Exception("Sensor device is null");

            _logger.Information("Starting sensor polling timer");
            if (_sensorDevice.isOnline())
                Update?.Invoke(this, _sensorDevice.get_currentValue());

            _handleYapiEventsTimer.Start();
        }

        public void StopPollTimer()
        {
            _logger.Information("Stopping sensor polling timer");
            _handleYapiEventsTimer.Stop();
        }

        private void TimedReport(YLightSensor sensor, YMeasure measure)
        {
            var currentValue = sensor.get_currentValue();
            Update?.Invoke(this, currentValue);

            lock (_historyLock)
            {
                if (ValueHistory.Count == 17280) ValueHistory.Dequeue();
                ValueHistory.Enqueue(new LuxReading(DateTime.Now, currentValue));
            }
        }

        private void HandleYapiEventsTimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            if (_sensorDevice == null || _sensorDevice.isOnline() == false)
            {
                Debug.Print("Offline");
                _sensorInitialized = false;
                return;
            }

            if (_sensorDevice.isOnline() && !_sensorInitialized)
            {
                _sensorDevice.stopDataLogger();
                _sensorDevice.set_logFrequency("OFF");
                _sensorDevice.clearCache();
                _sensorInitialized = true;
                // Update?.Invoke(this, _sensorDevice.get_currentValue());
            }

            YAPI.HandleEvents(ref _error);
            if (!string.IsNullOrEmpty(_error)) _logger.Error($"YAPI.HandleEvents error: {_error}");
        }
    }
}
