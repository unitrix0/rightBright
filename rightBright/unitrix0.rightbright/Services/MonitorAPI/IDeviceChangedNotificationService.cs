using System;

namespace unitrix0.rightbright.Services.MonitorAPI
{
    public interface IDeviceChangedNotificationService
    {
        event EventHandler DeviceChangedMessage;
    }
}