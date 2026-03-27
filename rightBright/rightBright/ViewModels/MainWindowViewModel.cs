using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Brightness;
using rightBright.Models.Sensors;
using rightBright.Services.Monitors;
using rightBright.Services.Sensors;
using rightBright.Services.SystemNotifications;
using rightBright.Settings;
using Serilog;

namespace rightBright.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMonitorEnummerationService _monitorsService = null!;
    private readonly ISensorService _sensorService = null!;
    private readonly IBrightnessController _brightnessController = null!;
    private readonly IMonitorChangedNotificationService _monitorChangedNotificationService = null!;
    private readonly ContentViewFactory _contentViewFactory = null!;
    private readonly ISettings _settings;
    private readonly ApplicationViewModel _applicationViewModel;

    private CancellationTokenSource? _refreshDisplaysCts;
    private readonly Lock _refreshDisplaysLock = new();
    private readonly SemaphoreSlim _refreshDisplaysSemaphore = new(1, 1);

    [ObservableProperty] private ObservableCollection<AmbientLightSensor> _availableSensors = [];

    [ObservableProperty] private ObservableCollection<DisplayInfo> _displays = [];

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ConnectSensorCommand))]
    private AmbientLightSensor? _selectedSensor;

    [ObservableProperty] private MainWindowContentViewModel _currentContent = null!;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ConnectSensorCommand))]
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

    public MainWindowViewModel(IMonitorEnummerationService monitorsService,
        ISensorService sensorService,
        IBrightnessController brightnessController,
        IMonitorChangedNotificationService monitorChangedNotificationService,
        ContentViewFactory contentViewFactory,
        ISettings settings,
        ApplicationViewModel applicationViewModel)
    {
        _monitorsService = monitorsService;
        _sensorService = sensorService;
        _brightnessController = brightnessController;
        _monitorChangedNotificationService = monitorChangedNotificationService;
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

        // Subscribe to BrightnessController's property changes to update SelectedSensor
        _brightnessController.PropertyChanged += OnBrightnessControllerPropertyChanged;

        _monitorChangedNotificationService.DeviceChangedMessage += (_, _) => ScheduleRefreshDisplays();
        _ = UpdateMonitors();
        UpdateSensors();
    }

    private void ScheduleRefreshDisplays()
    {
        lock (_refreshDisplaysLock)
        {
            _refreshDisplaysCts?.Cancel();
            _refreshDisplaysCts?.Dispose();
            _refreshDisplaysCts = new CancellationTokenSource();
            var token = _refreshDisplaysCts.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(500, token);
                    if (token.IsCancellationRequested) return;
                    Log.Information("[UI] Device changed -> refreshing Displays");
                    await RefreshDisplaysAsync();
                }
                catch (TaskCanceledException)
                {
                    // Expected for debouncing.
                }
            }, token);
        }
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

    private void OnBrightnessControllerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IBrightnessController.ConnectedSensor))
        {
            // Update SelectedSensor when BrightnessController connects to a sensor
            SelectedSensor = AvailableSensors
                .SingleOrDefault(s =>
                    s.SerialNumber == _brightnessController.ConnectedSensor?.SerialNumber);

            UpdateNoSelectionText(_sensorService.Error);
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
        await RefreshDisplaysAsync();
    }

    private async Task RefreshDisplaysAsync()
    {
        await _refreshDisplaysSemaphore.WaitAsync();
        try
        {
            // Capture selection before collection replacement so we can restore it after refresh.
            var oldSelection = SelectedScreenItem;
            var oldDeviceName = oldSelection?.DeviceName;
            var oldModelName = oldSelection?.ModelName;

            await Dispatcher.UIThread.InvokeAsync(() => _applicationViewModel.IsLoadingDisplays = true);

            List<DisplayInfo> displays;
            try
            {
                displays = await _monitorsService.GetDisplays();
            }
            catch
            {
                displays = [];
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Displays.Clear();
                foreach (var display in displays)
                {
                    Displays.Add(display);
                }
                
                SelectedScreenItem = Displays.SingleOrDefault(d => d.DeviceName == oldDeviceName) ??
                                     Displays.SingleOrDefault(d => d.ModelName == oldModelName);

                _applicationViewModel.IsLoadingDisplays = false;
                Log.Information("[UI] Displays refreshed. Count={Count}", Displays.Count);
            });
        }
        finally
        {
            _refreshDisplaysSemaphore.Release();
        }
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
            if (SelectedScreenItem != null)
            {
                var curveEditorViewModel =
                    (CurveEditorViewModel)_contentViewFactory.GetMainWindowContentViewModel<CurveEditorViewModel>();
                curveEditorViewModel.SelectedScreen = SelectedScreenItem;
                curveEditorViewModel.closeView += () => SelectedScreenItem = null;
                CurrentContent = curveEditorViewModel;
            }
            else
            {
                CurrentContent = new NoSelectionContentViewModel { Message = "Kein Bildschirm ausgewählt" };
            }
        }

        base.OnPropertyChanged(e);
    }
}
