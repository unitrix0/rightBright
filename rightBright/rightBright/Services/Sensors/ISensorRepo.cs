using System.Collections.Generic;
using rightBright.Sensors.Model;

namespace rightBright.Services.Sensors
{
    public interface ISensorRepo
    {
        List<AmbientLightSensor> GetSensors(bool forceUpdate = false);
    }
}