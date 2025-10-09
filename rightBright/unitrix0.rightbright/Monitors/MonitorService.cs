using System.Collections.ObjectModel;
using unitrix0.rightbright.Services.Logging;
using unitrix0.rightbright.Services.MonitorAPI;

namespace unitrix0.rightbright.Monitors
{
    public class MonitorService : IMonitorService
    {
        private readonly IMonitorEnummerationService _monitorEnummerationService;
        private readonly ILoggingService _logger;

        public ObservableCollection<DisplayInfo> Monitors { get; set; } = new ObservableCollection<DisplayInfo>();

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

        public void Clear()
        {
            Monitors.Clear();
        }

        public void Add()
        {
            Monitors.AddRange(_monitorEnummerationService.GetDisplays());
        }
    }
}
