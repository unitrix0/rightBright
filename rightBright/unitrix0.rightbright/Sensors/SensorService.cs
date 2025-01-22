using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using unitrix0.rightbright.Sensors.Model;
using unitrix0.rightbright.Services.Logging;
using unitrix0.rightbright.Settings;

namespace unitrix0.rightbright.Sensors
{
    public class SensorService : ISensorService
    {
        private readonly ISettings _settings;
        private readonly ILoggingService _logger;
        private YLightSensor? _sensorDevice;
        private string _error = "";
        private readonly Timer _handleYapiEventsTimer = new();
        private readonly Queue<Double> _valueHistory;
        private readonly ISensorRepo _sensorRepo;

        public event EventHandler<double>? Update;

        public string Error => _error;
        public string FriendlyName => _sensorDevice?.FriendlyName ?? "";
        public string Unit => _sensorDevice?.get_unit() ?? "";
        public bool Connected => _sensorDevice != null && _sensorDevice.isSensorReady() && _sensorDevice.isOnline();

        public Queue<double> ValueHistory => _valueHistory;

        public SensorService(ISensorRepo sensorRepo, ISettings settings, ILoggingService logger)
        {
            _sensorRepo = sensorRepo;
            _settings = settings;
            _logger = logger;
            _handleYapiEventsTimer.Elapsed += HandleYapiEventsTimerOnElapsed;
            _valueHistory = new Queue<double>(17280);

            YAPI.RegisterHub(_settings.HubUrl, ref _error);
            YAPI.UpdateDeviceList(ref _error);
        }


        public bool ConnectToSensor(string sensorFriendlyName)
        {
            _sensorDevice = YLightSensor.FindLightSensor(sensorFriendlyName);
            if (!_sensorDevice.isSensorReady() || !_sensorDevice.isOnline()) return false;
            _valueHistory.Clear();

            _sensorDevice.set_logFrequency("OFF");
            _sensorDevice.stopDataLogger();
            _sensorDevice.registerTimedReportCallback(TimedReport);
            _handleYapiEventsTimer.Interval = _settings.YapiEventsTimerInterval;

            return true;
        }

        public List<AmbientLightSensor> GetSensors(bool forceUpdate = false)
        {
            return _sensorRepo.GetSensors(forceUpdate);
        }

        public void StartPollTimer()
        {
            _logger.WriteInformation("Starting sensor polling timer");
            if (_sensorDevice == null || !Connected)
                throw new InvalidOperationException("Sensor not set or not connected");

            _sensorDevice.clearCache();
            _handleYapiEventsTimer.Start();
            Update?.Invoke(this, _sensorDevice.get_currentValue());
        }

        public void StopPollTimer()
        {
            _logger.WriteInformation("Stopping sensor polling timer");
            _handleYapiEventsTimer.Stop();
        }

        private void TimedReport(YLightSensor func, YMeasure measure)
        {
            var currentValue = func.get_currentValue();
            Update?.Invoke(this, currentValue);

            if (_valueHistory.Count == 17280) _valueHistory.Dequeue();
            _valueHistory.Enqueue(currentValue);
        }

        private void HandleYapiEventsTimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            YAPI.HandleEvents(ref _error);
        }
    }
}
