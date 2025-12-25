using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using rightBright.Brightness.Calculators;
using rightBright.Models.Monitors;
using rightBright.Services.CurveCalculation;
using rightBright.Services.Logging;
using rightBright.Settings;
using SkiaSharp;
using Timer = System.Timers.Timer;

namespace rightBright.ViewModels;

public partial class CurveEditorViewModel : MainWindowContentViewModel
{
    private readonly ICurveCalculationService _curveService;
    private readonly ILoggingService _logger;
    private readonly ISettings _settings;
    private readonly Timer _curveUpdateTimer;

    public Action? closeView;

    [ObservableProperty]
    private int _minBrightness;

    [ObservableProperty]
    private int _curve;

    [ObservableProperty]
    private double _progression;

    [ObservableProperty]
    private bool _active;

    [ObservableProperty]
    private ISeries[] _series =
    [
        new LineSeries<ObservablePoint>()
        {
            Name = "Current Curve",
            Stroke = new SolidColorPaint(SKColor.Parse("#BDBDBD"), 2),
            Fill = new SolidColorPaint(SKColor.Parse("#6BBDBDBD")),
            GeometryStroke = null,
            GeometrySize = 0,
            Values = new ObservableCollection<ObservablePoint>()
        },
        new LineSeries<ObservablePoint>()
        {
            Name = "New Curve",
            Stroke = new SolidColorPaint(SKColor.Parse("#5A60B4"), 2),
            Fill = new SolidColorPaint(SKColor.Parse("#6bBFC9FF")),
            GeometryStroke = new SolidColorPaint(SKColor.Parse("#5A60B4")),
            GeometrySize = 3,
            Values = new ObservableCollection<ObservablePoint>()
        }
    ];

    [ObservableProperty]
    private DisplayInfo? _selectedScreen;

    public CurveEditorViewModel()
    {
        _curveService = new CurveCalculationService(new ProgressiveBrightnessCalculator());
        _logger = new LoggingService();
        _curveUpdateTimer = new Timer();
        SeedDesignTimeData();
        _settings = null!;
    }

    public CurveEditorViewModel(ICurveCalculationService curveService, ILoggingService logger, ISettings settings)
    {
        _curveService = curveService;
        _logger = logger;
        _settings = settings;
        _curveUpdateTimer = new Timer(500);
        _curveUpdateTimer.Enabled = false;
        _curveUpdateTimer.AutoReset = false;
        _curveUpdateTimer.Elapsed += async (_, _) =>
            await CalculateCurve(MinBrightness, Curve, Progression, Series[1].Values);
    }

    [RelayCommand]
    private void RequestClose()
    {
        closeView?.Invoke();
    }

    [RelayCommand]
    private async Task ApplyCurve()
    {
        SelectedScreen!.CalculationParameters.MinBrightness = MinBrightness;
        SelectedScreen!.CalculationParameters.Curve = Curve;
        SelectedScreen!.CalculationParameters.Progression = Progression;
        SelectedScreen!.CalculationParameters.Active = Active;
        
        _settings.BrightnessCalculationParameters[SelectedScreen.ModelName] =
            SelectedScreen.CalculationParameters;
        _settings.Save();
        
        await CalculateCurrentCurve(SelectedScreen.CalculationParameters);
        Series[1].Values = null;
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
    {
        try
        {
            switch (eventArgs.PropertyName)
            {
                case nameof(SelectedScreen) when SelectedScreen != null:
                    MapCurentValuesToUi(SelectedScreen.CalculationParameters);
                    Series[1].Values = new ObservableCollection<ObservablePoint>();
                    
                    //Enable the update timer but keep it from running
                    _curveUpdateTimer.Enabled = true;
                    _curveUpdateTimer.Stop();

                    await CalculateCurrentCurve(SelectedScreen.CalculationParameters);
                    break;
                case nameof(MinBrightness):
                case nameof(Curve):
                case nameof(Progression):
                    _curveUpdateTimer.Stop();
                    _curveUpdateTimer.Start();
                    break;
            }

            base.OnPropertyChanged(eventArgs);
        }
        catch (Exception ex)
        {
            _logger.WriteError($"Error calcualting new monitor curve: {ex}");
        }
    }

    private async Task CalculateCurrentCurve(BrightnessCalculationParameters calculationParameters)
    {
        await CalculateCurve(calculationParameters.MinBrightness,
            calculationParameters.Curve, calculationParameters.Progression,
            Series[0].Values);
    }

    private void MapCurentValuesToUi(BrightnessCalculationParameters calculationParameters)
    {
        MinBrightness = calculationParameters.MinBrightness;
        Curve = calculationParameters.Curve;
        Progression = calculationParameters.Progression;
        Active = calculationParameters.Active;
    }

    private async Task CalculateCurve(int minBrightness, int curve, double progresssion,
        IEnumerable? seriesValues)
    {
        if (seriesValues is ObservableCollection<ObservablePoint> points)
        {
            points.Clear();
            await Task.Factory.StartNew(() => Thread.Sleep(250));
            _curveService.Calculate(minBrightness, curve, progresssion, 800)
                .ForEach(x => points.Add(new ObservablePoint(x.Item1, x.Item2)));
        }
    }

    private void SeedDesignTimeData()
    {
        SelectedScreen = new DisplayInfo()
        {
            ModelName = "Monitor 2",
            CalculationParameters = new BrightnessCalculationParameters()
            {
                Active = true,
                MinBrightness = 7,
                Curve = 25,
                Progression = 1.15
            }
        };
        
        Series[0].Values = _curveService.Calculate(SelectedScreen.CalculationParameters.MinBrightness,
                SelectedScreen.CalculationParameters.Curve, SelectedScreen.CalculationParameters.Progression, 800)
            .Select(x => new ObservablePoint(x.Item1, x.Item2))
            .ToList();

        MinBrightness = 7;
        Curve = 16;
        Progression = 1.18;
        Series[1].Values = _curveService.Calculate(MinBrightness, Curve, Progression, 800)
            .Select(x => new ObservablePoint(x.Item1, x.Item2))
            .ToArray();
    }
}
