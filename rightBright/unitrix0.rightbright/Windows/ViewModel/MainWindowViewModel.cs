using LiveCharts;
using LiveCharts.Defaults;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using unitrix0.rightbright.Brightness;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Sensors.Model;
using unitrix0.rightbright.Services.CurveCalculation;
using unitrix0.rightbright.Settings;

namespace unitrix0.rightbright.Windows.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IBrightnessController _brightnessController;
        private readonly IMonitorService _monitorService;
        private readonly ISettings _settings;
        private readonly ICurveCalculationService _curveCalculator;
        private AmbientLightSensor _selectedSensor;
        private bool _isSensorConnected;
        private BrightnessCalculationParameters _newCalculationParameters;
        private DisplayInfo _selectedMonitor;
        private IChartValues _currentCurve;
        private IChartValues _newCurve;

        public ObservableCollection<DisplayInfo> Monitors => _monitorService?.Monitors ?? new ObservableCollection<DisplayInfo>();

        public DisplayInfo SelectedMonitor
        {
            get => _selectedMonitor;
            set => SetProperty(ref _selectedMonitor, value, EditSelectedMonitor);
        }

        public BrightnessCalculationParameters NewCalculationParameters
        {
            get => _newCalculationParameters;
            set
            {
                SetProperty(ref _newCalculationParameters, value);
                RaisePropertyChanged(nameof(ShowMonitorSettings));
            }
        }
        public bool ShowMonitorSettings => NewCalculationParameters != null;

        public List<AmbientLightSensor> Sensors { get; }
        public AmbientLightSensor SelectedSensor
        {
            get => _selectedSensor;
            set
            {
                SetProperty(ref _selectedSensor, value);
                RaisePropertyChanged(nameof(IsSensorSelected));
                ConnectSensorCmd.RaiseCanExecuteChanged();
            }
        }
        public bool IsSensorSelected => SelectedSensor != null;
        public bool IsSensorConnected
        {
            get => _isSensorConnected;
            private set => SetProperty(ref _isSensorConnected, value, ConnectSensorCmd.RaiseCanExecuteChanged);
        }

        public IChartValues CurrentCurve
        {
            get => _currentCurve;
            set => SetProperty(ref _currentCurve, value);
        }

        public IChartValues NewCurve
        {
            get => _newCurve;
            set => SetProperty(ref _newCurve, value, ApplyNewCurve.RaiseCanExecuteChanged);
        }


        public DelegateCommand ConnectSensorCmd { get; }

        public DelegateCommand CloseDisplaySettings { get; }

        public DelegateCommand ApplyNewCurve { get; }

        // ReSharper disable once UnusedMember.Global
        public MainWindowViewModel()
        {
        }

        // ReSharper disable once UnusedMember.Global
        public MainWindowViewModel(IBrightnessController brightnessController, IMonitorService monitorService, ISensorRepo sensorRepo, ISettings settings, ICurveCalculationService curveCalculator)
        {
            ConnectSensorCmd = new DelegateCommand(ConnectSensor, () => IsSensorSelected && !IsSensorConnected);
            CloseDisplaySettings = new DelegateCommand(DeSelectMonitor, () => true);
            ApplyNewCurve = new DelegateCommand(SaveNewMonitorSettings, CanSaveNewMonitorSettings);

            _brightnessController = brightnessController;
            _monitorService = monitorService;
            _settings = settings;
            _curveCalculator = curveCalculator;

            Sensors = sensorRepo.GetSensors();
            SelectedSensor = _brightnessController.ConnectedSensor;

            IsSensorConnected = _brightnessController.ConnectedSensor != null;

            SaveSettingsOfNewMonitors();
        }

        private void DeSelectMonitor()
        {
            SelectedMonitor = null;
            NewCalculationParameters = null;
            CurrentCurve = null;
            NewCurve = null;
        }


        private void EditSelectedMonitor()
        {
            if (SelectedMonitor == null) return;

            NewCalculationParameters = new BrightnessCalculationParameters(SelectedMonitor.CalculationParameters);
            NewCalculationParameters.PropertyChanged += CreateCurveForNewSettings;

            NewCurve = new ChartValues<ObservablePoint>();
            CurrentCurve = _curveCalculator.Calculate(SelectedMonitor.CalculationParameters, SelectedSensor.MaxValue);
        }

        private void CreateCurveForNewSettings(object sender, PropertyChangedEventArgs e)
        {
            NewCurve = _curveCalculator.Calculate(NewCalculationParameters, SelectedSensor.MaxValue);
            Debug.Print(NewCurve.Count.ToString());
        }

        private void SaveNewMonitorSettings()
        {
            SelectedMonitor.CalculationParameters.MapFrom(NewCalculationParameters);
            EditSelectedMonitor();
            _settings.BrightnessCalculationParameters[SelectedMonitor.DeviceName] = SelectedMonitor.CalculationParameters;
            _settings.Save();
        }

        private bool CanSaveNewMonitorSettings()
        {
            //TODO More Validation
            return NewCurve?.Count > 0 || NewCalculationParameters?.Active != SelectedMonitor?.CalculationParameters.Active;
        }

        private void ConnectSensor()
        {
            IsSensorConnected = _brightnessController.ConnectSensor(SelectedSensor);
            if (IsSensorConnected) _settings.LastUsedSensor = SelectedSensor;
        }

        private void SaveSettingsOfNewMonitors()
        {
            foreach (var monitor in Monitors.Where(m => m.CalculationParameters.Active))
            {
                if (_settings.BrightnessCalculationParameters.ContainsKey(monitor.DeviceName)) continue;

                _settings.BrightnessCalculationParameters[monitor.DeviceName] = monitor.CalculationParameters;
            }

            _settings.Save();
        }
    }
}