using System;
using System.Collections.Generic;
using rightBright.Models.Sensors;

namespace rightBright.Services.Sensors
{
    public interface ISensorService
    {
        event EventHandler<double>? Update;
        bool SensorDeviceOnline { get; }
        string Error { get; }
        string FriendlyName { get; }
        string Unit { get; }
        Queue<LuxReading> ValueHistory { get; }
        LuxReading[] GetValueHistorySnapshot();
        bool ConnectToSensor(string sensorFriendlyName);
        void StartPollTimer();
        public List<AmbientLightSensor> GetSensors(bool forceUpdate = false);
        void StopPollTimer();
        void SetPollInterval(int milliseconds);
    }
}
