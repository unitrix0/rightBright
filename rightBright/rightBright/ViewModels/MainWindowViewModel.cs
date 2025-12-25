using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Brightness;
using rightBright.Models.Sensors;
using rightBright.Services.Monitors;
using rightBright.Services.Sensors;
using rightBright.Settings;

namespace rightBright.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMonitorEnummerationService _monitosService = null!;
    private readonly ISensorService _sensorService = null!;
    private readonly IBrightnessController _brightnessController = null!;
    private readonly ContentViewFactory _contentViewFactory = null!;
    private readonly ISettings _settings;
    private readonly ApplicationViewModel _applicationViewModel;

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

    public ApplicationViewModel ApplicationViewModel => _applicationViewModel;
    
    public bool IsNotLoadingDisplays => !_applicationViewModel.IsLoadingDisplays;

    public MainWindowViewModel()
    {
        if (Design.IsDesignMode) SeedDesignetimeData();
        _applicationViewModel = new ApplicationViewModel();
        _settings = new AppSettings();
    }

    public MainWindowViewModel(IMonitorEnummerationService monitosService,
        ISensorService sensorService,
        IBrightnessController brightnessController,
        ContentViewFactory contentViewFactory,
        ISettings settings,
        ApplicationViewModel applicationViewModel)
    {
        _monitosService = monitosService;
        _sensorService = sensorService;
        _brightnessController = brightnessController;
        _contentViewFactory = contentViewFactory;
        _settings = settings;
        _applicationViewModel = applicationViewModel;

        // Subscribe to ApplicationViewModel's property changes to update IsNotLoadingDisplays
        _applicationViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ApplicationViewModel.IsLoadingDisplays))
            {
                OnPropertyChanged(nameof(IsNotLoadingDisplays));
            }
        };

        _ = UpdateMonitors();
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

    private async Task UpdateMonitors()
    {
        try
        {
            _applicationViewModel.IsLoadingDisplays = true;

            List<DisplayInfo> displays = await _monitosService.GetDisplays();
            foreach (var display in displays)
            {
                Displays.Add(display);
            }
        }
        finally
        {
            _applicationViewModel.IsLoadingDisplays = false;
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
        if (!SensorConnected) return;
        
        _settings.LastUsedSensor = SelectedSensor!;
        _settings.Save();
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
            var curveEditorViewModel = (CurveEditorViewModel)_contentViewFactory.GetMainWindowContentViewModel<CurveEditorViewModel>();
            curveEditorViewModel.SelectedScreen = SelectedScreenItem!;
            curveEditorViewModel.closeView += () => CurrentContent = new NoSelectionContentViewModel();
            CurrentContent = curveEditorViewModel;
        }
        
        base.OnPropertyChanged(e);
    }
}
