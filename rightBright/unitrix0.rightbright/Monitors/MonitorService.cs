using System.Collections.Generic;
using System.Collections.ObjectModel;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Services.MonitorAPI;

namespace unitrix0.rightbright.Monitors
{
    public class MonitorService : IMonitorService
    {
        private readonly IMonitorEnummerationService _monitorEnummerationService;

        public ObservableCollection<DisplayInfo> Monitors { get; set; } = new ObservableCollection<DisplayInfo>();
        public MonitorService(IMonitorEnummerationService monitorEnummerationService)
        {
            _monitorEnummerationService = monitorEnummerationService;
        }

        public void UpdateList()
        {
            Monitors = new ObservableCollection<DisplayInfo>(_monitorEnummerationService.GetDisplays());
        }
    }
}