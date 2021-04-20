using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Prism.Commands;
using Prism.Mvvm;
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
        private readonly IMonitorService _monitorService;
        private readonly ISensorService _sensorService;
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


        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(IMonitorService monitorService, ISensorService sensorService, ISettings settings, ICurveCalculationService curveCalculator)
        {
            ConnectSensorCmd = new DelegateCommand(ConnectSensor, () => IsSensorSelected && !IsSensorConnected);
            CloseDisplaySettings = new DelegateCommand(DeSelectMonitor, () => true);
            ApplyNewCurve = new DelegateCommand(SaveNewCalculationSettings, CanSaveNewCalculationSettings);

            _monitorService = monitorService;
            _sensorService = sensorService;
            _settings = settings;
            _curveCalculator = curveCalculator;
            _monitorService.UpdateList();

            Sensors = _sensorService.GetSensors();
            _sensorService.Update += SensorServiceOnUpdate;

            TryConnectLastUsedSensor();
        }



        private void DeSelectMonitor()
        {
            SelectedMonitor = null;
            NewCalculationParameters = null;
            CurrentCurve = null;
            NewCurve = null;
        }

        private void TryConnectLastUsedSensor()
        {
            if (_settings.LastUsedSensor == null) return;

            SelectedSensor = Sensors.FirstOrDefault(s => s.FriendlyName == _settings.LastUsedSensor.FriendlyName);
            if (SelectedSensor == null) return;

            SelectedSensor.MaxValue = _settings.LastUsedSensor.MaxValue;
            SelectedSensor.MinValue = _settings.LastUsedSensor.MinValue;
            ConnectSensor();
        }

        private void SensorServiceOnUpdate(object sender, double e)
        {
            _selectedSensor.CurrentValue = (int)Math.Round(e);
        }

        private void EditSelectedMonitor()
        {
            if (SelectedMonitor == null) return;

            NewCalculationParameters = new BrightnessCalculationParameters(SelectedMonitor.CalculationParameters);
            NewCalculationParameters.PropertyChanged += CreateCurveForNewSettings;

            NewCurve = new ChartValues<Double>();
            CurrentCurve = _curveCalculator.Calculate(SelectedMonitor.CalculationParameters, SelectedSensor.MaxValue);
        }

        private void CreateCurveForNewSettings(object sender, PropertyChangedEventArgs e)
        {
            NewCurve = _curveCalculator.Calculate(NewCalculationParameters, SelectedSensor.MaxValue);
        }

        private void SaveNewCalculationSettings()
        {
            SelectedMonitor.CalculationParameters = new BrightnessCalculationParameters(NewCalculationParameters);
            EditSelectedMonitor();
        }
        private bool CanSaveNewCalculationSettings()
        {
            //TODO More Validation
            return NewCurve?.Count > 0;
        }

        private void ConnectSensor()
        {
            // TODO In BrightnessController verschieben
            IsSensorConnected = _sensorService.ConnectToSensor(SelectedSensor.FriendlyName);
            if (IsSensorConnected) _settings.LastUsedSensor = SelectedSensor;
        }
    }
}