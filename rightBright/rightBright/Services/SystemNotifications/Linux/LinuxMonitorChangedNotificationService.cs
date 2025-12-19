using System;
using rightBright.Services.DBus.ScreenBrightness;
using Tmds.DBus.Protocol;
using Address = Tmds.DBus.Address;

namespace rightBright.Services.SystemNotifications.Linux;

public class LinuxMonitorChangedNotificationService : IMonitorChangedNotificationService
{
    private readonly Connection _dbusSession;
    public event EventHandler? DeviceChangedMessage;

    public LinuxMonitorChangedNotificationService()
    {
        _dbusSession = new Connection(Address.Session);
        _dbusSession.ConnectAsync().GetAwaiter();
        WatchMonitorAdded();
    }

    private void WatchMonitorAdded()
    {
        try
        {
            var screenBrightnesSvc = new ScreenBrightnessService(_dbusSession, "org.kde.ScreenBrightness");
            var screenBrightness = screenBrightnesSvc.CreateScreenBrightness("/org/kde/ScreenBrightness");
            screenBrightness.WatchDisplayAddedAsync((exception, s) => DeviceChangedMessage?.Invoke(this, EventArgs.Empty));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
