using System.Collections.Generic;
using System.Diagnostics;
using Prism.Commands;
using Prism.Mvvm;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Sensors.Model;

namespace unitrix0.rightbright.Windows.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IMonitorService _monitorService;
        private readonly ISensorService _sensorService;
        private AmbientLightSensor _selectedSensor;
        private bool _isSensorConnected;
        public List<DisplayInfo> Monitors => _monitorService?.Monitors ?? new List<DisplayInfo>();

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

        public DelegateCommand ConnectSensorCmd { get; }

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(IMonitorService monitorService, ISensorService sensorService)
        {
            ConnectSensorCmd = new DelegateCommand(ConnectSensor, () => IsSensorSelected && !IsSensorConnected);

            _monitorService = monitorService;
            _sensorService = sensorService;
            _monitorService.UpdateList();

            Sensors = _sensorService.GetSensors();
        }

        private void ConnectSensor()
        {
            IsSensorConnected = _sensorService.ConnectToSensor(SelectedSensor.FriendlyName);
        }
    }
}