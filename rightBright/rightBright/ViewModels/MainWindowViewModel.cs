using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    private readonly IBrightnessController _brightnessController;

    [ObservableProperty]
    private ObservableCollection<AmbientLightSensor> _availableSensors = [];

    [ObservableProperty]
    private ObservableCollection<DisplayInfo> _displays = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectSensorCommand))]
    private AmbientLightSensor? _selectedSensor;

    [ObservableProperty]
    private MainViewContentViewModel _currentContent = null!;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectSensorCommand))]
    private bool _sensorConnected;


    public MainWindowViewModel()
    {
        if (Design.IsDesignMode) SeedDesignetimeData();
    }

    public MainWindowViewModel(IMonitorEnummerationService monitosService,
        ISensorService sensorService,
        IBrightnessController brightnessController)
    {
        _monitosService = monitosService;
        _sensorService = sensorService;
        _brightnessController = brightnessController;

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
            CurrentContent = new NoSelectionViewModel
            {
                Message = $"Sensoren konnte nicht abgefragt werden:\n {error}"
            };
            return;
        }

        if (AvailableSensors.Count == 0)
        {
            CurrentContent = new NoSelectionViewModel { Message = "Keine Sensoren gefunden" };
            return;
        }

        if (SelectedSensor == null)
        {
            CurrentContent = new NoSelectionViewModel { Message = "Kein Sensor verbunden" };
            return;
        }

        CurrentContent = new NoSelectionViewModel() { Message = "Kein Bildschirm ausgewählt" };
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
}
