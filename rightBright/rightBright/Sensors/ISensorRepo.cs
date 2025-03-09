using System.Collections.Generic;
using rightBright.Sensors.Model;

namespace unitrix0.rightbright.Sensors
{
    public interface ISensorRepo
    {
        List<AmbientLightSensor> GetSensors(bool forceUpdate = false);
    }
}