using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Win32;
using rightBright.Services.SystemNotifications.Windows.Constants;
using rightBright.WindowsApi.WindowMessages;
using rightBright.WindowsApi.WindowMessages.Enums;

namespace rightBright.Services.SystemNotifications.Windows
{
    public class WinMonitorChangedNotificationService : WindowsNotificationServiceBase, IMonitorChangedNotificationService
    {
        public event EventHandler? DeviceChangedMessage;

        [SupportedOSPlatform("windows")]
        public WinMonitorChangedNotificationService()
        {
            SystemEvents.DisplaySettingsChanged += (_, _) => Debug.WriteLine("@@@@@@@@@@@@@@@@ AppSettings Changed");

            RegisterClass(nameof(WinMonitorChangedNotificationService));
            var msgWinHandel = WindowMessageApiImports.CreateWindowEx(0, nameof(WinMonitorChangedNotificationService), "",
                0, 0, 0, 0, 0, HwinConstants.HWND_MESSAGE, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            DeviceChangeMessageHelper.RegisterMonitorDeviceNotification(msgWinHandel);
            DeviceChangeMessageHelper.RegisterUsbDeviceNotification(msgWinHandel);
        }

        protected override IntPtr WindowProc(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            Debug.Print($"Window Message: {(WindowMessages)message} params: 0x{wParam:X}");

            if ((WindowMessages)message != WindowMessages.DEVICECHANGE)
                return WindowMessageApiImports.DefWindowProc(hWnd, message, wParam, lParam);

            DeviceChangedMessage?.Invoke(this, EventArgs.Empty);
            return new IntPtr(1);
        }

        public override void Dispose()
        {
            DeviceChangeMessageHelper.UnRegisterMonitorDeviceNotification();
        }
    }
}
