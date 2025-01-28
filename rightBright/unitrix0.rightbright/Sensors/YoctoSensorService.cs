using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using unitrix0.rightbright.Sensors.Model;
using unitrix0.rightbright.Services.Logging;
using unitrix0.rightbright.Settings;

namespace unitrix0.rightbright.Sensors
{
    public class YoctoSensorService : ISensorService
    {
        private readonly ILoggingService _logger;
        private readonly Timer _handleYapiEventsTimer = new();
        private readonly ISensorRepo _sensorRepo;
        private YLightSensor? _sensorDevice;
        private string _error = "";
        private bool _sensorInitialized;

        public event EventHandler<double>? Update;

        public string Error => _error;
        public string FriendlyName => _sensorDevice?.FriendlyName ?? "";
        public string Unit => _sensorDevice?.get_unit() ?? "";
        public bool SensorDeviceOnline => _sensorDevice?.isOnline() ?? false;

        public Queue<double> ValueHistory { get; }

        public YoctoSensorService(ISensorRepo sensorRepo, ISettings settings, ILoggingService logger)
        {
            _sensorRepo = sensorRepo;
            _logger = logger;
            _handleYapiEventsTimer.Elapsed += HandleYapiEventsTimerOnElapsed;
            _handleYapiEventsTimer.Interval = settings.YapiEventsTimerInterval;
            ValueHistory = new Queue<double>(17280);

            YAPI.RegisterHub(settings.HubUrl, ref _error);
        }


        public bool ConnectToSensor(string sensorFriendlyName)
        {
            _sensorDevice = YLightSensor.FindLightSensor(sensorFriendlyName);
            _sensorDevice.registerTimedReportCallback(TimedReport);
            ValueHistory.Clear();
            return true;
        }

        public List<AmbientLightSensor> GetSensors(bool forceUpdate = false)
        {
            return _sensorRepo.GetSensors(forceUpdate);
        }

        public void StartPollTimer()
        {
            if (_sensorDevice == null) throw new Exception("Sensor device is null");
            
            _logger.WriteInformation("Starting sensor polling timer");
            _handleYapiEventsTimer.Start();
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

            if (ValueHistory.Count == 17280) ValueHistory.Dequeue();
            ValueHistory.Enqueue(currentValue);
            Debug.Print(currentValue.ToString());
        }

        private void HandleYapiEventsTimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            if(_sensorDevice == null || _sensorDevice.isOnline() == false)
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
            if (!string.IsNullOrEmpty(_error)) _logger.WriteError($"YAPI.HandleEvents error: {_error}");
        }
    }
}
