using System;

namespace unitrix0.rightbright.Services.MonitorAPI
{
    public interface IPowerNotificationService
    {
        event EventHandler ScreensPoweredOff;
        event EventHandler ScreensPoweredOn;
        event EventHandler SystemSuspending;
        event EventHandler SystemResuming;
    }
}