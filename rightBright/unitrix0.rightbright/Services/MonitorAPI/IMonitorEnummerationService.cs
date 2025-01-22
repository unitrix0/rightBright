using System.Collections.Generic;
using Prism.Common;
using unitrix0.rightbright.Monitors.Models;

namespace unitrix0.rightbright.Services.MonitorAPI
{
    public interface IMonitorEnummerationService
    {
        /// <summary>
        /// Returns informations about the connect Mointors
        /// </summary>
        /// <returns>collection of Display Info</returns>
        List<DisplayInfo> GetDisplays();

        /// <summary>
        /// Returns list of Devicenames and Monitor hnadles
        /// </summary>
        /// <returns></returns>
        List<MonitorHandleInfo> GetMonitorHandles();
    }
}
