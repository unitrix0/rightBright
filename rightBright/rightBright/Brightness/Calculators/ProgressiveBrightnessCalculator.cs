using System;

namespace rightBright.Brightness.Calculators
{
    public class ProgressiveBrightnessCalculator : IBrightnessCalculator
    {
        public double Calculate(double lux, double progression, int curve, int lowestBrightness)
        {
            return Math.Round(Math.Pow(lux / curve, progression) + lowestBrightness, 1);
        }
    }
}
