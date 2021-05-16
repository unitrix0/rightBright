using System.Collections.Generic;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Sensors
{
    public class SensorRepo : ISensorRepo
    {
        private List<AmbientLightSensor> _sensors;

        public SensorRepo()
        {
        }

        public List<AmbientLightSensor> GetSensors(bool forceUpdate = false)
        {
            if (_sensors != null && !forceUpdate) return _sensors;

            _sensors = new List<AmbientLightSensor>();
            var sensor = YLightSensor.FirstSensor();

            while (sensor != null)
            {

                if (sensor.get_functionId() == "lightSensor")
                {
                    _sensors.Add(new AmbientLightSensor(sensor));
                }

                sensor = sensor.nextSensor();
            }

            return _sensors;
        }
    }
}