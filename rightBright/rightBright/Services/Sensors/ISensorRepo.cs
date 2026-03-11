using System.Collections.Generic;
using rightBright.Models.Sensors;

namespace rightBright.Services.Sensors
{
    public interface ISensorRepo
    {
        List<AmbientLightSensor> GetSensors(bool forceUpdate = false);
    }
}