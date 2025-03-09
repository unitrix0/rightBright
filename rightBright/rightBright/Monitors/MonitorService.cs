using System.Collections.Generic;
using System.Collections.ObjectModel;
using rightBright.Services.MonitorAPI;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Services.Logging;

namespace rightBright.Monitors
{
    public class MonitorService : IMonitorService
    {
        private readonly IMonitorEnummerationService _monitorEnummerationService;
        private readonly ILoggingService _logger;

        public List<DisplayInfo> Monitors { get; set; } = [];

        public MonitorService(IMonitorEnummerationService monitorEnummerationService, ILoggingService logger)
        {
            _monitorEnummerationService = monitorEnummerationService;
            _logger = logger;
        }

        public void UpdateList()
        {
            _logger.WriteInformation($"{nameof(MonitorService)} Updaing Monitors list");
            Monitors.Clear();
            foreach (var displayInfo in _monitorEnummerationService.GetDisplays())
            {
                Monitors.Add(displayInfo);
            }
        }
    }
}
