using System;
using System.Collections.Generic;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Sensors
{
    public interface ISensorService
    {
        event EventHandler<double> Update;
        bool Connected { get; }
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