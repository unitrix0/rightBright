using System;
using System.Collections.Generic;
using System.Linq;
using rightBright.Services.DBus.ddcutil;
using Tmds.DBus.Protocol;
using Address = Tmds.DBus.Address;

namespace rightBright.Services.Monitors;

public class LinuxMonitorEnumService : IMonitorEnummerationService
{
    private readonly Connection _dbusSession;
    private const string BusName = "com.ddcutil.DdcutilService";
    private List<DisplayInfo> _monitors = [];

    public LinuxMonitorEnumService()
    {
        _dbusSession = new Connection(Address.Session);
    }

    public List<DisplayInfo> GetDisplays()
    {
        try
        {
            if (_monitors.Count > 0) return _monitors;

            _dbusSession.ConnectAsync().GetAwaiter();
            var ddcutilSvc = new DdcutilService(_dbusSession, BusName);
            var ddcutil = ddcutilSvc.CreateDdcutilInterface("/com/ddcutil/DdcutilObject");

            var detected = ddcutil.DetectAsync(0x0).GetAwaiter().GetResult();
            _monitors = detected.DetectedDisplays.Select(detectedDisplay => new DisplayInfo()
            {
                DeviceName = detectedDisplay.Item5,
                ModelName = detectedDisplay.Item5,
            }).ToList();
        
            return _monitors;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];
        }
    }
}
