using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32;
using rightBright.Services.SystemNotifications.Windows.Constants;
using rightBright.Services.SystemNotifications.Windows.Structs;
using rightBright.WindowsApi.WindowMessages;
using rightBright.WindowsApi.WindowMessages.Enums;

namespace rightBright.Services.SystemNotifications.Windows
{
    public class PowerNotificationService : NotificationServiceBase, IPowerNotificationService
    {
        public event EventHandler? ScreensPoweredOff;
        public event EventHandler? ScreensPoweredOn;

        public event EventHandler? SystemSuspending;
        public event EventHandler? SystemResuming;

        [SupportedOSPlatform("windows")]
        public PowerNotificationService()
        {
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
            RegisterClass(nameof(PowerNotificationService));
            var msgWinHandel = WindowMessageApiImports.CreateWindowEx(0, nameof(PowerNotificationService), "", 0, 0, 0,
                0, 0, HwinConstants.HWND_MESSAGE, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            DeviceChangeMessageHelper.RegisterPowerSettingNotification(msgWinHandel);
        }

        protected override IntPtr WindowProc(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            Debug.Print($"Window Message: {(WindowMessages)message} params: 0x{wParam:X}");

            switch ((WindowMessages)message)
            {
                case WindowMessages.POWERBROADCAST:
                    HandlePowerBroadcastMsg(Marshal.PtrToStructure<PowerbroadcastSetting>(lParam));
                    return new IntPtr(1);
                default:
                    return WindowMessageApiImports.DefWindowProc(hWnd, message, wParam, lParam);
            }
        }

        private void HandlePowerBroadcastMsg(PowerbroadcastSetting msgParams)
        {
            if (msgParams.PowerSetting != PowerSettingGuids.GUID_MONITOR_POWER_ON)
            {
                Debug.Print($"\t\t*** < {PowerSettingGuids.Names[msgParams.PowerSetting]} ({msgParams.Data})");
                return;
            }

            Debug.Print($"\t\t{PowerSettingGuids.Names[msgParams.PowerSetting]}: {msgParams.Data}");

            switch (msgParams.Data)
            {
                case 1:
                    ScreensPoweredOn?.Invoke(this, EventArgs.Empty);
                    break;
                case 0:
                    ScreensPoweredOff?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    SystemResuming?.Invoke(this, EventArgs.Empty);
                    break;
                case PowerModes.Suspend:
                    SystemSuspending?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        public override void Dispose()
        {
            DeviceChangeMessageHelper.UnregisterPowerSettingNotification();
        }
    }
}
