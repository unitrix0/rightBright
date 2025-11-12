using System;
using System.Collections.Generic;
using System.Linq;
using rightBright.Services.DBus.ddcutil;
using rightBright.Services.DBus.ScreenBrightness;
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
        _dbusSession.ConnectAsync().GetAwaiter();
    }

    public List<DisplayInfo> GetDisplays()
    {
        try
        {
            if (_monitors.Count > 0) return _monitors;


            var ddcutilSvc = new DdcutilService(_dbusSession, BusName);
            var ddcutil = ddcutilSvc.CreateDdcutilInterface("/com/ddcutil/DdcutilObject");

            var detected = ddcutil.DetectAsync(0x0).GetAwaiter().GetResult();
            _monitors = detected.DetectedDisplays.OrderBy(x => x.Item1)
                .Select(detectedDisplay =>
                {
                    string[] parts = [detectedDisplay.Item5.ToString(), detectedDisplay.Item6.ToString()];
                    return new DisplayInfo()
                    {
                        DeviceName = detectedDisplay.Item1.ToString(),
                        ModelName = $"{string.Join('-', parts)}",
                    };
                }).ToList();

            return _monitors;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return [];
        }
    }

    private void WatchMonitorAdded()
    {
        try
        {
            var screenBrightnesSvc = new ScreenBrightnessService(_dbusSession, "org.kde.ScreenBrightness");
            var screenBrightness = screenBrightnesSvc.CreateScreenBrightness("/org/kde/ScreenBrightness");
            screenBrightness.WatchDisplayAddedAsync((exception, s) =>
            {
                _monitors.Clear();
                GetDisplays();
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
