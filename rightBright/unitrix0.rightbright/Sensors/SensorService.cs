using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using unitrix0.rightbright.Sensors.Model;
using unitrix0.rightbright.Settings;

namespace unitrix0.rightbright.Sensors
{
    public class SensorService : ISensorService
    {
        private readonly ISettings _settings;
        private YLightSensor _sensorDevice;
        private string _error = "";
        private readonly Timer _handleYapiEventsTimer = new();
        private readonly Queue<Double> _valueHistory;
        private readonly ISensorRepo _sensorRepo;

        public event EventHandler<double> Update;

        public string Error => _error;
        public string FriendlyName => _sensorDevice?.FriendlyName ?? "";
        public string Unit => _sensorDevice?.get_unit() ?? "";
        public bool Connected => _sensorDevice.isSensorReady() && _sensorDevice.isOnline();

        public Queue<double> ValueHistory => _valueHistory;

        public SensorService(ISensorRepo sensorRepo, ISettings settings)
        {
            _sensorRepo = sensorRepo;
            _settings = settings;
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
            YAPI.HandleEvents(ref _error);
            _handleYapiEventsTimer.Start();
        }

        private void TimedReport(YLightSensor func, YMeasure measure)
        {
            Update?.Invoke(this, measure.get_averageValue());

            if (_valueHistory.Count == 17280) _valueHistory.Dequeue();
            _valueHistory.Enqueue(measure.get_averageValue());
        }

        private void HandleYapiEventsTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            YAPI.HandleEvents(ref _error);
        }
    }
}