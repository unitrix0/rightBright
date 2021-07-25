using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using unitrix0.rightbright.Services.MonitorAPI.Constants;
using unitrix0.rightbright.Services.MonitorAPI.Structs;

namespace unitrix0.rightbright.Services.MonitorAPI
{
    public static class DeviceChangeMessageHelper
    {
        /// <summary>System detected a new device</summary>
        public const int DbtDeviceArrival = 0x8000;
        /// <summary>Device is gone</summary>
        public const int DbtDeviceRemoveComplete = 0x8004;
        /// <summary>Device change event</summary>
        public const int WmDeviceChange = 0x0219;
        private const int DbtDeviceTypeDeviceInterface = 5;

        /// <summary>
        /// USB devices
        /// <remarks>https://docs.microsoft.com/en-us/windows-hardware/drivers/install/guid-devinterface-usb-device</remarks>
        /// </summary>
        private static readonly Guid GuidDeviceInterfaceUsbDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // 

        /// <summary>
        /// Monitor devices
        /// https://docs.microsoft.com/en-us/windows-hardware/drivers/install/guid-devinterface-monitor
        /// </summary>
        private static readonly Guid GuidDeviceInterfaceMonitorDevice = new Guid("E6F07B5F-EE97-4a90-B076-33F57BF4EAA7"); // 
        private static IntPtr _usbNotificationHandle;
        private static IntPtr _monitorNotificationHandle;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);
        
        [DllImport(@"User32", SetLastError=true, EntryPoint = "RegisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid powerSettingGuid, Int32 flags);

        [DllImport(@"User32", EntryPoint = "UnregisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)] 
        private static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        public static bool IsMonitor(IntPtr lParam)
        {
            return IsDeviceOfClass(lParam, GuidDeviceInterfaceMonitorDevice);
        }

        public static bool IsUsbDevice(IntPtr lParam)
        {
            return IsDeviceOfClass(lParam, GuidDeviceInterfaceUsbDevice);
        }

        /// <summary>
        ///  Registers a window to receive notifications when Monitor devices are plugged or unplugged.
        /// </summary>
        /// <param name="windowHandle"></param>
        public static void RegisterMonitorDeviceNotification(IntPtr windowHandle)
        {
            var dbi= CreateBroadcastDeviceInterface(GuidDeviceInterfaceMonitorDevice);
            _monitorNotificationHandle = RegisterDeviceNotification(dbi, windowHandle);
        }

        /// <summary>
        /// Registers a window to receive notifications when USB devices are plugged or unplugged.
        /// </summary>
        /// <param name="windowHandle"></param>
        public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
        {
            var dbi = CreateBroadcastDeviceInterface(GuidDeviceInterfaceUsbDevice);
            _usbNotificationHandle = RegisterDeviceNotification(dbi, windowHandle);
        }

        public static void RegisterPowerSettingNotification(IntPtr windowHandle)
        {
            RegisterPowerSettingNotification(windowHandle, ref PowerSettingGuids.GUID_MONITOR_POWER_ON, 0);
            RegisterPowerSettingNotification(windowHandle, ref PowerSettingGuids.GUID_CONSOLE_DISPLAY_STATE, 0);
            RegisterPowerSettingNotification(windowHandle, ref PowerSettingGuids.GUID_SESSION_DISPLAY_STATUS, 0);
            RegisterPowerSettingNotification(windowHandle, ref PowerSettingGuids.GUID_ACDC_POWER_SOURCE, 0);
        }

        public static void UnregisterPowerSettingNotification()
        {
            UnregisterPowerSettingNotification(_monitorNotificationHandle);
        }

        /// <summary>
        /// UnRegisters the window for Monitor device notifications
        /// </summary>
        public static void UnRegisterMonitorDeviceNotification()
        {
            UnregisterDeviceNotification(_monitorNotificationHandle);
        }

        /// <summary>
        /// UnRegisters the window for USB device notifications
        /// </summary>
        public static void UnRegisterUsbDeviceNotification()
        {
            UnregisterDeviceNotification(_usbNotificationHandle);
        }

        private static bool IsDeviceOfClass(IntPtr lParam, Guid classGuid)
        {
            var hdr = Marshal.PtrToStructure<DevBroadcastDeviceInterface>(lParam);

            if (hdr.DeviceType != DbtDeviceTypeDeviceInterface)
                return false;

            var devIf = Marshal.PtrToStructure<DevBroadcastDeviceInterface>(lParam);

            return devIf.ClassGuid == classGuid;

        }

        private static DevBroadcastDeviceInterface CreateBroadcastDeviceInterface(Guid classGuid)
        {
            var dbi = new DevBroadcastDeviceInterface
            {
                DeviceType = DbtDeviceTypeDeviceInterface,
                Reserved = 0,
                ClassGuid = classGuid,
                Name = 0
            };

            dbi.Size = Marshal.SizeOf(dbi);

            return dbi;
        }

        private static IntPtr RegisterDeviceNotification(DevBroadcastDeviceInterface dbi, IntPtr windowHandle)
        {
            var buffer = Marshal.AllocHGlobal(dbi.Size);
            IntPtr handle;

            try
            {
                Marshal.StructureToPtr(dbi, buffer, true);

                handle = RegisterDeviceNotification(windowHandle, buffer, 0);
            }
            finally
            {
                // Free buffer
                Marshal.FreeHGlobal(buffer);
            }

            return handle;
        }
    }
}