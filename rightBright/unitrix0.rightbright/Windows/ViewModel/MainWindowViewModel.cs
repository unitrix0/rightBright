using System.Collections.Generic;
using Prism.Mvvm;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Monitors.Models;

namespace unitrix0.rightbright.Windows.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IMonitorService _monitorService;
        public List<DisplayInfo> Monitors => _monitorService?.Monitors ?? new List<DisplayInfo>();

        public MainWindowViewModel()
        {
            
        }

        public MainWindowViewModel(IMonitorService monitorService)
        {
            _monitorService = monitorService;
            _monitorService.UpdateList();
        }
    }
}