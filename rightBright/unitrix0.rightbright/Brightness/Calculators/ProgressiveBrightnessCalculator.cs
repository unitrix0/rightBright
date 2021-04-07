using System;
using unitrix0.rightbright.Sensors;

namespace unitrix0.rightbright.Brightness.Calculators
{
    public class ProgressiveBrightnessCalculator : IBrightnessCalculator
    {

        public int Calculate(double lux, double progression, int lowestBrightness)
        {
            return (int)Math.Round(Math.Pow(lux / 100, progression) + lowestBrightness);
        }
    }
}