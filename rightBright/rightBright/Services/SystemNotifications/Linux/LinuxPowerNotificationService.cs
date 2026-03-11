using System;

namespace rightBright.Services.SystemNotifications.Linux;

public class LinuxPowerNotificationService : IPowerNotificationService
{
    public event EventHandler? ScreensPoweredOff;
    public event EventHandler? ScreensPoweredOn;
    public event EventHandler? SystemSuspending;
    public event EventHandler? SystemResuming;
}
