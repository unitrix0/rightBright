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
            int x = 0;
            
            do
            {
                var item = _brightnessCalculator.Calculate(x, calculationParameters.Progression, calculationParameters.Curve, calculationParameters.MinBrightness);
                values.Add(item);
                x++;
            } while (x < maxLuxValue);
            
            return values;
        }
    }
}