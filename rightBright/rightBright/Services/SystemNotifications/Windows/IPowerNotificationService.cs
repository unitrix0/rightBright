using System;

namespace rightBright.Services.SystemNotifications.Windows
{
    public interface IPowerNotificationService
    {
        event EventHandler ScreensPoweredOff;
        event EventHandler ScreensPoweredOn;
        event EventHandler SystemSuspending;
        event EventHandler SystemResuming;
    }
}