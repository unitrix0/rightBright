using System.Linq;
using unitrix0.rightbright.Brightness.Calculators;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Services.Brightness;

namespace unitrix0.rightbright.Brightness
{
    public class BrightnessController
    {
        private readonly ISetBrightnessService _brightnessService;
        private readonly IMonitorService _monitorService;
        private readonly IBrightnessCalculator _brightnessCalculator;

        public BrightnessController(ISensorService sensor, ISetBrightnessService brightnessService, IMonitorService monitorService, IBrightnessCalculator brightnessCalculator)
        {
            _brightnessService = brightnessService;
            _monitorService = monitorService;
            _brightnessCalculator = brightnessCalculator;

            sensor.Update += SensorOnUpdate;
        }

        private void SensorOnUpdate(object sender, double e)
        {
            var monitors = _monitorService.Monitors.Where(m => m.Active);
            foreach (var monitor in monitors)
            {
                var newBrightness = _brightnessCalculator.Calculate(e, 2.105d, 20);
                _brightnessService.SetBrightness(monitor.Handle, newBrightness);
            }
        }
    }
}