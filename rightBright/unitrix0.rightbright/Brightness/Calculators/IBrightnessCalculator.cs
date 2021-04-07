﻿namespace unitrix0.rightbright.Brightness.Calculators
{
    public interface IBrightnessCalculator
    {
        int Calculate(double lux, double progression, int lowestBrightness);
    }
}