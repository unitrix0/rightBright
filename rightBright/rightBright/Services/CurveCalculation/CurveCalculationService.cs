using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using rightBright.Brightness.Calculators;
using rightBright.Models.Monitors;

namespace rightBright.Services.CurveCalculation
{
    public class CurveCalculationService : ICurveCalculationService
    {
        private readonly IBrightnessCalculator _brightnessCalculator;

        public CurveCalculationService(IBrightnessCalculator brightnessCalculator)
        {
            _brightnessCalculator = brightnessCalculator;
        }

        public List<Tuple<int, double>> Calculate(int minBrightness, int curve, double progresssion, int maxLuxValue)
        {
            var values = new List<Tuple<int, double>>();
            var step = maxLuxValue / 50;
            var x = 0;
            double brightness;

            do
            {
                brightness = _brightnessCalculator.Calculate(x, progresssion, curve, minBrightness);
            
                values.Add(new Tuple<int, double>(x, brightness > 100 ? 100 : brightness));
                x += step;
            } while (brightness < 100);

            if (x < maxLuxValue) values.Add(new Tuple<int, double>(maxLuxValue, 100));

            return values;
        }
    }
}
