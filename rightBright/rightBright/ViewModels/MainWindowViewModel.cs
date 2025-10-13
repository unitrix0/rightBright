using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Brightness;
using rightBright.Models.Sensors;
using rightBright.Services.Monitors;
using rightBright.Services.Sensors;

namespace rightBright.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMonitorEnummerationService _monitosService = null!;
    private readonly ISensorService _sensorService = null!;
    private readonly IBrightnessController _brightnessController = null!;
    private readonly ContentViewFactory _contentViewFactory = null!;

    [ObservableProperty]
    private ObservableCollection<AmbientLightSensor> _availableSensors = [];

    [ObservableProperty]
    private ObservableCollection<DisplayInfo> _displays = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectSensorCommand))]
    private AmbientLightSensor? _selectedSensor;

    [ObservableProperty]
    private MainWindowContentViewModel _currentContent = null!;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectSensorCommand))]
    private bool _sensorConnected;

    [ObservableProperty] private string _selectedScreenDeviceName = "";

    [ObservableProperty] private DisplayInfo? _selectedScreenItem;

    public MainWindowViewModel()
    {
        if (Design.IsDesignMode) SeedDesignetimeData();
    }

    public MainWindowViewModel(IMonitorEnummerationService monitosService,
        ISensorService sensorService,
        IBrightnessController brightnessController,
        ContentViewFactory contentViewFactory)
    {
        _monitosService = monitosService;
        _sensorService = sensorService;
        _brightnessController = brightnessController;
        _contentViewFactory = contentViewFactory;

        UpdateMonitors();
        UpdateSensors();
    }

    private void UpdateSensors()
    {
        try
        {
            foreach (var lightSensor in _sensorService.GetSensors())
            {
                AvailableSensors.Add(lightSensor);
            }

            SelectedSensor = AvailableSensors
                .SingleOrDefault(s =>
                    s.SerialNumber == _brightnessController.ConnectedSensor?.SerialNumber);
        
            UpdateNoSelectionText(_sensorService.Error);
        }
        catch (Exception ex)
        {
            UpdateNoSelectionText(ex.Message);
        }
    }

    private void UpdateNoSelectionText(string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            CurrentContent = new NoSelectionContentViewModel
            {
                Message = $"Sensoren konnte nicht abgefragt werden:\n {error}"
            };
            return;
        }

        if (AvailableSensors.Count == 0)
        {
            CurrentContent = new NoSelectionContentViewModel { Message = "Keine Sensoren gefunden" };
            return;
        }

        if (SelectedSensor == null)
        {
            CurrentContent = new NoSelectionContentViewModel { Message = "Kein Sensor verbunden" };
            return;
        }

        CurrentContent = new NoSelectionContentViewModel() { Message = "Kein Bildschirm ausgewählt" };
    }

    private void UpdateMonitors()
    {
        foreach (var display in _monitosService.GetDisplays())
        {
            Displays.Add(display);
        }
    }

    [RelayCommand]
    private void EditMonitorSettings(int monitorId)
    {
    }

    [RelayCommand(CanExecute = nameof(CanConnectSensor))]
    private void ConnectSensor()
    {
        SensorConnected = _brightnessController.Run(SelectedSensor!);
        UpdateNoSelectionText(_sensorService.Error);
    }

    private bool CanConnectSensor()
    {
        return SelectedSensor != null && _brightnessController.ConnectedSensor == null && !SensorConnected;
    }


    private void SeedDesignetimeData()
    {
        AvailableSensors =
        [
            new AmbientLightSensor()
            {
                FriendlyName = "LIGHTMK3-117AE3E.lightSensor",
                MinValue = 10,
                CurrentValue = 55,
                MaxValue = 123
            }
        ];

        SelectedSensor = AvailableSensors[0];
        Displays =
        [
            new DisplayInfo() { DeviceName = "Screen 1", ModelName = "Model" },
            new DisplayInfo() { DeviceName = "Screen 2", ModelName = "Model" }
        ];
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedScreenItem))
        {
            CurrentContent = _contentViewFactory.GetMainWindowContentViewModel<CurveEditorContentViewModel>();
            ((CurveEditorContentViewModel)CurrentContent).SelectedDisplay = SelectedScreenItem!;
        }
        
        base.OnPropertyChanged(e);
    }
}
