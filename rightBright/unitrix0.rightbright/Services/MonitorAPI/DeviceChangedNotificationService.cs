using Microsoft.Win32;
using System;
using System.Diagnostics;
using unitrix0.rightbright.Services.MonitorAPI.Constants;
using unitrix0.rightbright.Services.WindowMessageApi;
using unitrix0.rightbright.Services.WindowMessageApi.Enums;

namespace unitrix0.rightbright.Services.MonitorAPI
{
    public class DeviceChangedNotificationService : NotificationServiceBase, IDeviceChangedNotificationService
    {
        public event EventHandler? DeviceChangedMessage;

        public DeviceChangedNotificationService()
        {
            SystemEvents.DisplaySettingsChanged += (_, _) => Debug.WriteLine("@@@@@@@@@@@@@@@@ Settings Changed");

            RegisterClass(nameof(DeviceChangedNotificationService));
            var msgWinHandel = WindowMessageApiImports.CreateWindowEx(0, nameof(DeviceChangedNotificationService), "", 0, 0, 0, 0, 0, HwinConstants.HWND_MESSAGE, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            DeviceNotificationHelper.RegisterMonitorDeviceNotification(msgWinHandel);
        }

        protected override IntPtr WindowProc(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            Debug.Print($"Window Message: {(WindowMessages)message} params: 0x{wParam:X}");
            
            switch ((WindowMessages)message)
            {
                case WindowMessages.DEVICECHANGE:
                    Debug.Print($"\t\tDeviceChange");
                    DeviceChangedMessage?.Invoke(this, EventArgs.Empty);
                    return new IntPtr(1);
                default:
                    return WindowMessageApiImports.DefWindowProc(hWnd, message, wParam, lParam);
            }

            
        }

        public override void Dispose()
        {
            DeviceNotificationHelper.UnRegisterMonitorDeviceNotification();
        }
    }
}