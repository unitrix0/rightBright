using LiveCharts;
using LiveCharts.Defaults;
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

        public ChartValues<ObservablePoint> Calculate(BrightnessCalculationParameters calculationParameters,
            int maxLuxValue)
        {
            var values = new ChartValues<ObservablePoint>();
            var step = maxLuxValue / 50;
            var x = 0;
            double brightness;

            do
            {
                brightness = _brightnessCalculator.Calculate(x, calculationParameters.Progression,
                    calculationParameters.Curve, calculationParameters.MinBrightness);

                values.Add(new ObservablePoint(x, brightness > 100 ? 100 : brightness));
                x += step;
            } while (brightness < 100);

            if (x < maxLuxValue) values.Add(new ObservablePoint(maxLuxValue, 100));

            return values;
        }
    }
}