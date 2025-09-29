using System;
using System.Collections.Generic;
using rightBright.Models.Sensors;

namespace rightBright.Services.Sensors
{
    public class SensorRepo : ISensorRepo
    {
        private List<AmbientLightSensor>? _sensors;

        public List<AmbientLightSensor> GetSensors(bool forceUpdate = false)
        {
            if (_sensors != null && !forceUpdate) return _sensors;

            var error = "";
            YAPI.UpdateDeviceList(ref error);
            if (!string.IsNullOrEmpty(error)) throw new Exception(error);
            
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
