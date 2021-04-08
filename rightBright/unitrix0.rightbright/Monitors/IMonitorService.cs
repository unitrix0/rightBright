﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using unitrix0.rightbright.Monitors.Models;

namespace unitrix0.rightbright.Monitors
{
    public interface IMonitorService
    {
        List<DisplayInfo> Monitors { get; set; }
        void UpdateList();
    }
}