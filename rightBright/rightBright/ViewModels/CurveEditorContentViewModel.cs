using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using rightBright.Services.CurveCalculation;

namespace rightBright.ViewModels;

public partial class CurveEditorContentViewModel : MainWindowContentViewModel
{
    [ObservableProperty] private ObservableCollection<int> _currentCurve = [];
    [ObservableProperty] private ObservableCollection<int> _newCurve = [];
    [ObservableProperty] private DisplayInfo _selectedDisplay;

    public CurveEditorContentViewModel()
    {
    }

    public CurveEditorContentViewModel(ICurveCalculationService curveService)
    {
        CurrentCurve =curveService.Calculate()
    }
}
