using System.Collections.Generic;
using System.Threading.Tasks;
using unitrix0.rightbright.Monitors.Models;

namespace rightBright.Services.Monitors
{
    public interface IMonitorEnummerationService
    {
        /// <summary>
        /// Returns informations about the connect Mointors
        /// </summary>
        /// <returns>collection of Display Info</returns>
        Task<List<DisplayInfo>> GetDisplays();
    }
}
