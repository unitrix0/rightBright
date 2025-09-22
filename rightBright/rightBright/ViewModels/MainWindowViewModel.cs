using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Models.Sensors;
using rightBright.Services.Monitors;
using rightBright.Services.Sensors;
using rightBright.Views;

namespace rightBright.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMonitorEnummerationService _monitosService = null!;
    private readonly ISensorService _sensorService = null!;

    [ObservableProperty] private ObservableCollection<AmbientLightSensor> _availableSensors = [];
    [ObservableProperty] private ObservableCollection<DisplayInfo> _displays = [];
    [ObservableProperty] private AmbientLightSensor? _selectedSensor;
    [ObservableProperty] private MainViewContentViewModel _currentContent = null!;
    

    public MainWindowViewModel()
    {
        if (Design.IsDesignMode) SeedDesigntimeData();
    }

    public MainWindowViewModel(IMonitorEnummerationService monitosService, ISensorService sensorService)
    {
        _monitosService = monitosService;
        _sensorService = sensorService;

        UpdateMonitors();
        UpdateSensors();
    }

    private void UpdateSensors()
    {
        foreach (var lightSensor in _sensorService.GetSensors())
        {
            AvailableSensors.Add(lightSensor);
        }

        if (string.IsNullOrEmpty(_sensorService.Error))
        {
            CurrentContent = AvailableSensors.Count == 0
                ? new NoSelectionViewModel { Message = "Keine Sensoren gefunden" }
                : new NoSelectionViewModel { Message = "Kein Sensor ausgewählt" };
        }
        else
        {
            CurrentContent = new NoSelectionViewModel
            {
                Message = $"Sensoren konnte nicht abgefragt werden:\n {_sensorService.Error}"
            };
        }
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

    private void SeedDesigntimeData()
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
