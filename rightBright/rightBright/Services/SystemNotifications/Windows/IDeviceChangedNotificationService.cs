using System;

namespace rightBright.Services.SystemNotifications.Windows
{
    public interface IDeviceChangedNotificationService
    {
        event EventHandler DeviceChangedMessage;
        event EventHandler UsbDeviceConnectedMessage;
        event EventHandler UsbDeviceDisconnectedMessage;
    }
}