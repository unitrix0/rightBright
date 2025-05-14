using System.Collections.Generic;

namespace rightBright.Services.Monitors
{
    public interface IMonitorService
    {
        void UpdateList();
        List<DisplayInfo> Monitors { get; set; }
    }
}
