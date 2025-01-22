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
        public event EventHandler? UsbDeviceConnectedMessage;
        public event EventHandler? UsbDeviceDisconnectedMessage;

        public DeviceChangedNotificationService()
        {
            SystemEvents.DisplaySettingsChanged += (_, _) => Debug.WriteLine("@@@@@@@@@@@@@@@@ Settings Changed");

            RegisterClass(nameof(DeviceChangedNotificationService));
            var msgWinHandel = WindowMessageApiImports.CreateWindowEx(0, nameof(DeviceChangedNotificationService), "",
                0, 0, 0, 0, 0, HwinConstants.HWND_MESSAGE, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            DeviceChangeMessageHelper.RegisterMonitorDeviceNotification(msgWinHandel);
            DeviceChangeMessageHelper.RegisterUsbDeviceNotification(msgWinHandel);
        }

        protected override IntPtr WindowProc(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            Debug.Print($"Window Message: {(WindowMessages)message} params: 0x{wParam:X}");

            if ((WindowMessages)message != WindowMessages.DEVICECHANGE)
                return WindowMessageApiImports.DefWindowProc(hWnd, message, wParam, lParam);


            var isUsbDev = DeviceChangeMessageHelper.IsUsbDevice(lParam);
            Debug.Print($"\t\tDeviceChange USB:{isUsbDev}");

            if (isUsbDev)
            {
                HandleUsbDeviceChange((DeviceChangeEventTypes)wParam);
                return new IntPtr(1);
            }

            DeviceChangedMessage?.Invoke(this, EventArgs.Empty);
            return new IntPtr(1);
        }

        private void HandleUsbDeviceChange(DeviceChangeEventTypes eventType)
        {
            if (eventType == DeviceChangeEventTypes.DBT_DEVICEARRIVAL)
                UsbDeviceConnectedMessage?.Invoke(this, EventArgs.Empty);
            else
                UsbDeviceDisconnectedMessage?.Invoke(this, EventArgs.Empty);
        }

        public override void Dispose()
        {
            DeviceChangeMessageHelper.UnRegisterMonitorDeviceNotification();
            DeviceChangeMessageHelper.UnRegisterUsbDeviceNotification();
        }
    }
}
