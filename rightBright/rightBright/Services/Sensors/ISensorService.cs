using System;
using System.Collections.Generic;
using rightBright.Models.Sensors;

namespace rightBright.Services.Sensors
{
    public interface ISensorService
    {
        event EventHandler<double> Update;
        bool SensorDeviceOnline { get; }
        string Error { get; }
        string FriendlyName { get; }
        string Unit { get; }
        Queue<double> ValueHistory { get; }
        bool ConnectToSensor(string sensorFriendlyName);
        void StartPollTimer();
        public List<AmbientLightSensor> GetSensors(bool forceUpdate = false);
        void StopPollTimer();
    }
}
