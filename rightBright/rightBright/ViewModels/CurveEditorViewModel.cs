using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Defaults;
using rightBright.Models.Monitors;
using rightBright.Services.CurveCalculation;
using unitrix0.rightbright.Brightness.Calculators;

namespace rightBright.ViewModels;

public partial class CurveEditorViewModel : MainWindowContentViewModel
{
    [ObservableProperty]
    private ObservablePoint[]? _currentCurve;

    [ObservableProperty]
    private ObservablePoint[]? _newCurve;

    [ObservableProperty]
    private DisplayInfo _selectedMonitor;

    public CurveEditorViewModel()
    {
        SelectedMonitor = new DisplayInfo(){ModelName = "Monitor 2"};
        SeedDesignTimeCurve();
    }

    public CurveEditorViewModel(ICurveCalculationService curveService, MainWindowViewModel mainViewModel)
    {
        SelectedMonitor = mainViewModel.SelectedScreenItem!;
        var calcParams = new BrightnessCalculationParameters(mainViewModel.SelectedScreenItem!.CalculationParameters);
        CurrentCurve = curveService.Calculate(calcParams, mainViewModel.SelectedSensor?.MaxValue ?? 800)
            .Select(x => new ObservablePoint(x.Item1, x.Item2))
            .ToArray();
    }

    private void SeedDesignTimeCurve()
    {
        var calcService = new CurveCalculationService(new ProgressiveBrightnessCalculator());
        var calcParams = new BrightnessCalculationParameters()
        {
            Curve = 12,
            MinBrightness = 7,
            Progression = 1.14
        };
        var calcParamsNew = new BrightnessCalculationParameters()
        {
            Curve = 6,
            MinBrightness = 7,
            Progression = 1.0
        };

        CurrentCurve = calcService.Calculate(calcParams, 800)
            .Select(x => new ObservablePoint(x.Item1, x.Item2))
            .ToArray();

        NewCurve = calcService.Calculate(calcParamsNew, 800)
            .Select(x => new ObservablePoint(x.Item1, x.Item2))
            .ToArray();
    }
}
