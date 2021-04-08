using System.Collections.Generic;
using System.Collections.ObjectModel;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Services.MonitorAPI;

namespace unitrix0.rightbright.Monitors
{
    public class MonitorService : IMonitorService
    {
        private readonly IMonitorEnummerationService _monitorEnummerationService;

        public List<DisplayInfo> Monitors { get; set; } = new List<DisplayInfo>();
        public MonitorService(IMonitorEnummerationService monitorEnummerationService)
        {
            _monitorEnummerationService = monitorEnummerationService;
        }

        public void UpdateList()
        {
            Monitors = _monitorEnummerationService.GetDisplays();
        }
    }
}