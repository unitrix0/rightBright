using System;
using System.Diagnostics;
using LiveCharts;
using unitrix0.rightbright.Brightness.Calculators;
using unitrix0.rightbright.Monitors.Models;

namespace unitrix0.rightbright.Services.CurveCalculation
{
    public class CurveCalculationService : ICurveCalculationService
    {
        private readonly IBrightnessCalculator _brightnessCalculator;

        public CurveCalculationService(IBrightnessCalculator brightnessCalculator)
        {
            _brightnessCalculator = brightnessCalculator;
        }

        public ChartValues<double> Calculate(BrightnessCalculationParameters calculationParameters, int maxLuxValue)
        {
            var values = new ChartValues<double>();
            double item;
            int x = 0;
            
            do
            {
                item = _brightnessCalculator.Calculate(x, calculationParameters.Progression, calculationParameters.Curve, calculationParameters.MinBrightness);
                values.Add(item);
                x++;
            } while (item < 100 && x < 100000);
            
            return values;
        }
    }
}