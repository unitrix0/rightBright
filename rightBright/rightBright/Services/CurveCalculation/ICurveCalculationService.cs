using System;
using System.Collections.Generic;
using rightBright.Models.Monitors;

namespace rightBright.Services.CurveCalculation
{
    public interface ICurveCalculationService
    {
        List<Tuple<int, double>> Calculate(int minBrightness, int curve, double progresssion, int maxLuxValue);
    }
}
