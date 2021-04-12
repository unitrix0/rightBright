using System;
using System.Collections.Generic;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Sensors
{
    public interface ISensorService
    {
        event EventHandler<double> Update;
        List<AmbientLightSensor> GetSensors();
        bool ConnectToSensor(string sensorFriendlyName);
    }
}