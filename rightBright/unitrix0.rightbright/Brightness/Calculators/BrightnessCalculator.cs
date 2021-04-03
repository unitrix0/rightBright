using System;
using unitrix0.rightbright.Sensors;

namespace unitrix0.rightbright.Brightness.Calculators
{
    public class BrightnessCalculator
    {
        public int LowestBrightness { get; set; }
        public double Progression { get; set; }

        public int Brightness { get; private set; }

        public BrightnessCalculator(ISensorService sensor)
        {
            sensor.Update += _sensor_Update;
        }

        private void _sensor_Update(object sender, double lux)
        {
            Brightness = (int) Math.Round(Math.Pow(lux / 100, Progression) + LowestBrightness);
        }
    }
}