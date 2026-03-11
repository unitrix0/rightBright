using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Models.Monitors;
using rightBright.Services.Logging;
using rightBright.Settings;

namespace rightBright.ViewModels;

public partial class CurveEditorViewModel : MainWindowContentViewModel
{
    private readonly ILoggingService _logger;
    private readonly ISettings _settings;

    public Action? closeView;

    [ObservableProperty]
    private int _minBrightness;

    [ObservableProperty]
    private double _controlPointX = 400;

    [ObservableProperty]
    private double _controlPointY = 50;

    [ObservableProperty]
    private int _maxLux = 800;

    [ObservableProperty]
    private bool _active;

    [ObservableProperty]
    private int _savedMinBrightness;

    [ObservableProperty]
    private double _savedControlPointX = 400;

    [ObservableProperty]
    private double _savedControlPointY = 50;

    [ObservableProperty]
    private int _savedMaxLux = 800;

    [ObservableProperty]
    private bool _showSavedCurve;

    [ObservableProperty]
    private DisplayInfo? _selectedScreen;

    public CurveEditorViewModel()
    {
        _logger = new LoggingService();
        SeedDesignTimeData();
        _settings = null!;
    }

    public CurveEditorViewModel(ILoggingService logger, ISettings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    [RelayCommand]
    private void RequestClose()
    {
        closeView?.Invoke();
    }

    [RelayCommand]
    private void ApplyCurve()
    {
        SelectedScreen!.CalculationParameters.MinBrightness = MinBrightness;
        SelectedScreen!.CalculationParameters.ControlPointX = ControlPointX;
        SelectedScreen!.CalculationParameters.ControlPointY = ControlPointY;
        SelectedScreen!.CalculationParameters.MaxLux = MaxLux;
        SelectedScreen!.CalculationParameters.Active = Active;

        _settings.BrightnessCalculationParameters[SelectedScreen.ModelName] =
            SelectedScreen.CalculationParameters;
        _settings.Save();

        SnapshotSavedCurve(SelectedScreen.CalculationParameters);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
    {
        try
        {
            if (eventArgs.PropertyName == nameof(SelectedScreen) && SelectedScreen != null)
            {
                MapCurrentValuesToUi(SelectedScreen.CalculationParameters);
                SnapshotSavedCurve(SelectedScreen.CalculationParameters);
            }

            base.OnPropertyChanged(eventArgs);
        }
        catch (Exception ex)
        {
            _logger.WriteError($"Error in curve editor property change: {ex}");
        }
    }

    private void SnapshotSavedCurve(BrightnessCalculationParameters p)
    {
        SavedMinBrightness = p.MinBrightness;
        SavedControlPointX = p.ControlPointX;
        SavedControlPointY = p.ControlPointY;
        SavedMaxLux = p.MaxLux;
        ShowSavedCurve = true;
    }

    private void MapCurrentValuesToUi(BrightnessCalculationParameters p)
    {
        MinBrightness = p.MinBrightness;
        ControlPointX = p.ControlPointX;
        ControlPointY = p.ControlPointY;
        MaxLux = p.MaxLux;
        Active = p.Active;
    }

    private void SeedDesignTimeData()
    {
        SelectedScreen = new DisplayInfo
        {
            ModelName = "Monitor 2",
            CalculationParameters = new BrightnessCalculationParameters
            {
                Active = true,
                MinBrightness = 7,
                ControlPointX = 200,
                ControlPointY = 60,
                MaxLux = 800
            }
        };

        MinBrightness = 7;
        ControlPointX = 300;
        ControlPointY = 45;
        MaxLux = 800;
    }
}
