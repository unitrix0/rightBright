using System;
using System.Collections.Generic;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Sensors
{
    public class SensorRepo : ISensorRepo
    {
        private List<AmbientLightSensor>? _sensors;


        public List<AmbientLightSensor> GetSensors(bool forceUpdate = false)
        {
            if (_sensors != null && !forceUpdate) return _sensors;

            var errmsg = "";
            YAPI.UpdateDeviceList(ref errmsg);
            if (!string.IsNullOrEmpty(errmsg)) throw new Exception($"Error getting sensors: {errmsg}");
            
            _sensors = new List<AmbientLightSensor>();
            var sensor = YLightSensor.FirstLightSensor();

            while (sensor != null)
            {
                if (sensor.get_functionId() == "lightSensor")
                {
                    _sensors.Add(new AmbientLightSensor(sensor));
                }

                sensor = sensor.nextLightSensor();
            }

            return _sensors;
        }
    }
}
