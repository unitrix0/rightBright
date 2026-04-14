using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;
using Microsoft.Win32;
using rightBright.Services.SystemNotifications.Windows.Constants;
using rightBright.WindowsApi.WindowMessages;
using rightBright.WindowsApi.WindowMessages.Enums;

namespace rightBright.Services.SystemNotifications.Windows
{
    [SupportedOSPlatform("windows")]
    public class WinMonitorChangedNotificationService : WindowsNotificationServiceBase, IMonitorChangedNotificationService
    {
        /// <summary>
        /// After the last <see cref="WindowMessages.DEVICECHANGE"/>, wait this long before raising
        /// <see cref="DeviceChangedMessage"/> so the display stack can settle (RDP, dock, etc.).
        /// </summary>
        private const int DeviceChangeDebounceMilliseconds = 1000;

        private readonly Timer _deviceChangeDebounceTimer;
        private volatile bool _disposed;

        public event EventHandler? DeviceChangedMessage;

        public WinMonitorChangedNotificationService()
        {
            _deviceChangeDebounceTimer = new Timer(OnDeviceChangeDebounceElapsed, null, Timeout.Infinite,
                Timeout.Infinite);

            SystemEvents.DisplaySettingsChanged += (_, _) => Debug.WriteLine("@@@@@@@@@@@@@@@@ AppSettings Changed");

            RegisterClass(nameof(WinMonitorChangedNotificationService));
            var msgWinHandel = WindowMessageApiImports.CreateWindowEx(0, nameof(WinMonitorChangedNotificationService), "",
                0, 0, 0, 0, 0, HwinConstants.HWND_MESSAGE, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            DeviceChangeMessageHelper.RegisterMonitorDeviceNotification(msgWinHandel);
            DeviceChangeMessageHelper.RegisterUsbDeviceNotification(msgWinHandel);
        }

        private void OnDeviceChangeDebounceElapsed(object? state)
        {
            if (_disposed) return;
            try
            {
                DeviceChangedMessage?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeviceChangedMessage subscriber threw: {ex}");
            }
        }

        protected override IntPtr WindowProc(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            Debug.Print($"Window Message: {(WindowMessages)message} params: 0x{wParam:X}");

            if ((WindowMessages)message != WindowMessages.DEVICECHANGE)
                return WindowMessageApiImports.DefWindowProc(hWnd, message, wParam, lParam);

            _deviceChangeDebounceTimer.Change(DeviceChangeDebounceMilliseconds, Timeout.Infinite);
            return new IntPtr(1);
        }

        public override void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _deviceChangeDebounceTimer.Dispose();
            DeviceChangeMessageHelper.UnRegisterMonitorDeviceNotification();
        }
    }
}
