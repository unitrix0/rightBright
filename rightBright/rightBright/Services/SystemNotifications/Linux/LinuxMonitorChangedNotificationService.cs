using System;

namespace rightBright.Services.SystemNotifications.Linux;

public class LinuxMonitorChangedNotificationService : IMonitorChangedNotificationService
{
    public event EventHandler? DeviceChangedMessage;
}
