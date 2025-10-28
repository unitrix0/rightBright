using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using rightBright.Models.Monitors;
using rightBright.Services.CurveCalculation;
using rightBright.Services.Sensors;

namespace rightBright.ViewModels;

public partial class CurveEditorContentViewModel : MainWindowContentViewModel
{
    [ObservableProperty]
    private IChartValues? _currentCurve;

    [ObservableProperty]
    private IChartValues? _newCurve;

    public CurveEditorContentViewModel()
    {
    }

    public CurveEditorContentViewModel(ICurveCalculationService curveService, MainWindowViewModel mainViewModel)
    {
        var calcParams = new BrightnessCalculationParameters(mainViewModel.SelectedScreenItem!.CalculationParameters);
        CurrentCurve = curveService.Calculate(calcParams, mainViewModel.SelectedSensor!.MaxValue);
    }
}
