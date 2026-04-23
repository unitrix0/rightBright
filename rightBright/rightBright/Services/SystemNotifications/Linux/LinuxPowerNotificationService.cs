using System;

namespace rightBright.Services.SystemNotifications.Linux;

public class LinuxPowerNotificationService : IPowerNotificationService
{
    public event EventHandler? ScreensPoweredOff
    {
        add { }
        remove { }
    }

    public event EventHandler? ScreensPoweredOn
    {
        add { }
        remove { }
    }

    public event EventHandler? SystemSuspending
    {
        add { }
        remove { }
    }

    public event EventHandler? SystemResuming
    {
        add { }
        remove { }
    }
}
