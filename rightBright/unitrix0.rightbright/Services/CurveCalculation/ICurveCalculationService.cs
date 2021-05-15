using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Defaults;
using unitrix0.rightbright.Monitors.Models;

namespace unitrix0.rightbright.Services.CurveCalculation
{
    public interface ICurveCalculationService
    {
        ChartValues<ObservablePoint> Calculate(BrightnessCalculationParameters calculationParameters, int maxLuxValue);
    }
}