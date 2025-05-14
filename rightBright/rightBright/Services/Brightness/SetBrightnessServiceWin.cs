using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using rightBright.Services.Logging;
using rightBright.Services.Monitors;
using rightBright.WindowsApi;
using rightBright.WindowsApi.Monitor.Structs;
using unitrix0.rightbright.Monitors.Models;

namespace rightBright.Services.Brightness
{
    public class SetBrightnessServiceWin : ISetBrightnessService
    {
        private readonly ILoggingService _logger;
        private readonly IMonitorEnummerationService _monitorEnummerationService;
        private uint _minValue;
        private uint _maxValue;


        public SetBrightnessServiceWin(ILoggingService logger, IMonitorEnummerationService monitorEnummerationService)
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
            var getMonBrightness = DxvaImports.GetMonitorBrightness(monitors[0].hPhysicalMonitor, ref _minValue,
                ref currentBrightness, ref _maxValue);
            if (!getMonBrightness)
            {
                var lastWin32Error = Marshal.GetLastWin32Error();
                var ex = new Win32Exception(lastWin32Error);
                if (ex.HResult != -2147467259)
                {
                    _logger.WriteError(
                        $"{nameof(DxvaImports.GetMonitorBrightness)} for {monitor.DeviceName} ({lastWin32Error}) Error: 0x{ex.HResult:X} {ex.Message} - {ex.HResult}");
                    _logger.WriteError($"Returned brightness: {currentBrightness}");
                }
            }

            if (currentBrightness == newValue) return;

            var result = DxvaImports.SetMonitorBrightness(monitors[0].hPhysicalMonitor, (uint)newValue);
            Debug.Print($"{nameof(SetBrightness)} FROM:{currentBrightness} TO: {newValue} => success: {result}");
        }

        private PHYSICAL_MONITOR[] GetPhysicalMonitors(IntPtr ptr)
        {
            var physicalMonitorCount = GetPhysicalMonitorCount(ptr);
            var monitors = new PHYSICAL_MONITOR[physicalMonitorCount];

            if (DxvaImports.GetPhysicalMonitorsFromHMONITOR(ptr, physicalMonitorCount, monitors)) return monitors;

            var error = Marshal.GetLastWin32Error();
            throw new Exception($"Cannot get phisical monitor handle! {error}");
        }

        private uint GetPhysicalMonitorCount(IntPtr ptr)
        {
            uint count = 0;
            if (DxvaImports.GetNumberOfPhysicalMonitorsFromHMONITOR(ptr, ref count)) return count;

            var error = Marshal.GetLastWin32Error();
            throw new Exception($"Cannot get monitor count! ({error})");
        }
    }
}
