using LiveCharts;
using LiveCharts.Defaults;
using rightBright.Models.Monitors;

namespace rightBright.Services.CurveCalculation
{
    public interface ICurveCalculationService
    {
        ChartValues<ObservablePoint> Calculate(BrightnessCalculationParameters calculationParameters, int maxLuxValue);
    }
}
