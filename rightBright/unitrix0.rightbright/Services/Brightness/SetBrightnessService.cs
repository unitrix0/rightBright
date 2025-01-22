using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Services.Logging;
using unitrix0.rightbright.Services.MonitorAPI;
using unitrix0.rightbright.Services.MonitorAPI.Structs;

namespace unitrix0.rightbright.Services.Brightness
{
    public class SetBrightnessService : ISetBrightnessService
    {
        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor,
            ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize,
            [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness,
            ref uint currentBrightness,
            ref uint maxBrightness);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice,
            uint dwFlags);

        private readonly ILoggingService _logger;
        private readonly IMonitorEnummerationService _monitorEnummerationService;
        private uint _minValue;
        private uint _maxValue;


        public SetBrightnessService(ILoggingService logger, IMonitorEnummerationService monitorEnummerationService)
        {
            _logger = logger;
            _monitorEnummerationService = monitorEnummerationService;
        }

        public void SetBrightness(DisplayInfo monitor, int newValue)
        {
            newValue = newValue > 100 ? 100 : newValue;
            newValue = newValue < 1 ? 1 : newValue;
            var currentBrightness = (_maxValue - _minValue) * (uint)newValue / 100u + _minValue;

            List<MonitorHandleInfo> monitorHandles = _monitorEnummerationService.GetMonitorHandles();
            var monitorHandleInfo = monitorHandles.Single(h => h.DeviceName == monitor.DeviceName);

            PHYSICAL_MONITOR[] monitors = GetPhysicalMonitors(monitorHandleInfo.Handle);
            var getMonBrightness = GetMonitorBrightness(monitors[0].hPhysicalMonitor, ref _minValue,
                ref currentBrightness, ref _maxValue);
            if (!getMonBrightness)
            {
                var lastWin32Error = Marshal.GetLastWin32Error();
                var ex = new Win32Exception(lastWin32Error);
                if (ex.HResult != -2147467259)
                {
                    _logger.WriteError(
                        $"{nameof(GetMonitorBrightness)} for {monitor.DeviceName} ({lastWin32Error}) Error: 0x{ex.HResult:X} {ex.Message} - {ex.HResult}");
                    _logger.WriteError($"Returned brightness: {currentBrightness}");
                }
            }

            if (currentBrightness == newValue) return;

            var result = SetMonitorBrightness(monitors[0].hPhysicalMonitor, (uint)newValue);
            Debug.Print($"{nameof(SetBrightness)} FROM:{currentBrightness} TO: {newValue} => success: {result}");
        }

        private PHYSICAL_MONITOR[] GetPhysicalMonitors(IntPtr ptr)
        {
            var physicalMonitorCount = GetPhysicalMonitorCount(ptr);
            var monitors = new PHYSICAL_MONITOR[physicalMonitorCount];

            if (GetPhysicalMonitorsFromHMONITOR(ptr, physicalMonitorCount, monitors)) return monitors;

            var error = Marshal.GetLastWin32Error();
            throw new Exception($"Cannot get phisical monitor handle! {error}");
        }

        private uint GetPhysicalMonitorCount(IntPtr ptr)
        {
            uint count = 0;
            if (GetNumberOfPhysicalMonitorsFromHMONITOR(ptr, ref count)) return count;

            var error = Marshal.GetLastWin32Error();
            throw new Exception($"Cannot get monitor count! ({error})");
        }
    }
}
