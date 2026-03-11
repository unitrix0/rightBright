using System;

namespace rightBright.Services.SystemNotifications
{
    public interface IMonitorChangedNotificationService
    {
        event EventHandler DeviceChangedMessage;
    }
}
