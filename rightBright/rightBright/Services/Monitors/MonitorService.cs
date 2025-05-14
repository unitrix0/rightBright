using System.Collections.Generic;
using rightBright.Services.Logging;
using rightBright.WindowsApi.Monitor;

namespace rightBright.Services.Monitors
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
