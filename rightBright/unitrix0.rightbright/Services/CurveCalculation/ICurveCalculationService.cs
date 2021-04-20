using LiveCharts;
using unitrix0.rightbright.Monitors.Models;

namespace unitrix0.rightbright.Services.CurveCalculation
{
    public interface ICurveCalculationService
    {
        ChartValues<double> Calculate(BrightnessCalculationParameters calculationParameters, int maxLuxValue);
    }
}