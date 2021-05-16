using System.Collections.Generic;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Sensors
{
    public interface ISensorRepo
    {
        List<AmbientLightSensor> GetSensors(bool forceUpdate = false);
    }
}