using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using rightBright.Services.DBus.ddcutil;
using Serilog;
using rightBright.Services.SystemNotifications;
using Tmds.DBus.Protocol;
using Address = Tmds.DBus.Address;

namespace rightBright.Services.Monitors.Enummerators;

public class LinuxMonitorEnumService : IMonitorEnummerationService
{
    private readonly ILogger _logger;
    private readonly IMonitorChangedNotificationService _monitorChangedNotificationService;
    private readonly Connection _dbusSession;
    private const string BusName = "com.ddcutil.DdcutilService";
    private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
    private List<DisplayInfo> _monitors = [];

    public LinuxMonitorEnumService(ILogger logger, IMonitorChangedNotificationService monitorChangedNotificationService)
    {
        _logger = logger;
        
        _monitorChangedNotificationService = monitorChangedNotificationService;
        _monitorChangedNotificationService.DeviceChangedMessage += (_, _) => _ = Update();
        
        _dbusSession = new Connection(Address.Session);
        _dbusSession.ConnectAsync().GetAwaiter();
    }

    public async Task<List<DisplayInfo>> GetDisplays()
    {
        await _cacheLock.WaitAsync();
        try
        {
            if (_monitors.Count > 0) return _monitors;

            var ddcutilSvc = new DdcutilService(_dbusSession, BusName);
            var ddcutil = ddcutilSvc.CreateDdcutilInterface("/com/ddcutil/DdcutilObject");

            var detected = await ddcutil.DetectAsync(0x0);
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
            _logger.Error($"Error getting displays: {ex.Message}");
            return [];
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private async Task Update()
    {
        await _cacheLock.WaitAsync();
        try
        {
            _monitors.Clear();
            
            // Repopulate the cache
            var ddcutilSvc = new DdcutilService(_dbusSession, BusName);
            var ddcutil = ddcutilSvc.CreateDdcutilInterface("/com/ddcutil/DdcutilObject");

            var detected = await ddcutil.DetectAsync(0x0);
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
        }
        catch (Exception ex)
        {
            _logger.Error($"Error updating displays cache: {ex.Message}");
        }
        finally
        {
            _cacheLock.Release();
        }
    }
}
