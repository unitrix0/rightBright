using System;
using System.Timers;

namespace unitrix0.rightbright.Sensors
{
    public class SensorService : ISensorService
    {
        private readonly YLightSensor _sensor;
        private string _error = "";
        private readonly Timer _handleYapiEventsTimer = new Timer();

        public event EventHandler<double> Update;

        public string Error => _error;
        public string FriendlyName => _sensor?.FriendlyName ?? "";
        public string Unit => _sensor?.get_unit() ?? "";
        public bool IsReady => _sensor?.isSensorReady() ?? false;

        public SensorService()
        {
            _handleYapiEventsTimer.Interval = 5000; // TODO Parameter
            _handleYapiEventsTimer.Elapsed += HandleYapiEventsTimerOnElapsed;

            YAPI.RegisterHub("USB", ref _error);
            YAPI.UpdateDeviceList(ref _error);
            
            _sensor = (YLightSensor)YSensor.FindSensor("LIGHTMK3-17AE3E"); // TODO Parameter
            _sensor.registerTimedReportCallback(TimedReport);
        }

        private void TimedReport(YLightSensor func, YMeasure measure)
        {
            Update?.Invoke(this, measure.get_averageValue());
        }

        private void HandleYapiEventsTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            YAPI.HandleEvents(ref _error);
        }
    }
}