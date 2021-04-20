using System;
using unitrix0.rightbright.Sensors;

namespace unitrix0.rightbright.Brightness.Calculators
{
    public class ProgressiveBrightnessCalculator : IBrightnessCalculator
    {

        public double Calculate(double lux, double progression, int curve, int lowestBrightness)
        {
            return Math.Round(Math.Pow(lux / curve, progression) + lowestBrightness, 1);
        }
    }
}