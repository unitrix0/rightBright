using System.Collections.Generic;
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
        public List<DisplayInfo> Monitors => _monitorService?.Monitors ?? new List<DisplayInfo>();

        public List<AmbientLightSensor> Sensors { get; }

        public AmbientLightSensor SelectedSensor { get; set; }

        public MainWindowViewModel()
        {
            
        }

        public MainWindowViewModel(IMonitorService monitorService, ISensorService sensorService)
        {
            _monitorService = monitorService;
            _sensorService = sensorService;
            _monitorService.UpdateList();

            Sensors = _sensorService.GetSensors();

        }
    }
}