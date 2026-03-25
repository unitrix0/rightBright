using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Serilog;
using rightBright.WindowsApi;
using rightBright.WindowsApi.Monitor;
using rightBright.WindowsApi.Monitor.Structs;
using unitrix0.rightbright.Monitors.Models;

namespace rightBright.Services.Brightness
{
    public class SetBrightnessServiceWin : ISetBrightnessService
    {
        private readonly ILogger _logger;


        public SetBrightnessServiceWin(ILogger logger)
        {
            _logger = logger;
        }

        public Task<bool> SetBrightness(DisplayInfo monitor, int newValue)
        {
            return Task.Factory.StartNew(() => SetBrightnessInternal(monitor, newValue));
        }

        private bool SetBrightnessInternal(DisplayInfo monitor, int newValue)
        {
            _logger.Information($"[SetBrightness:Win] '{monitor.ModelName}' (dev={monitor.DeviceName}): requested={newValue}%");

            uint maxValue = 0;
            uint minValue = 0;
            newValue = newValue > 100 ? 100 : newValue;
            newValue = newValue < 1 ? 1 : newValue;
            uint currentBrightness = 0;

            List<MonitorHandleInfo> monitorHandles = GetMonitorHandles();
            var monitorHandleInfo = monitorHandles.Single(h => h.DeviceName == monitor.DeviceName);

            PHYSICAL_MONITOR[] monitors = GetPhysicalMonitors(monitorHandleInfo.Handle);
            var getMonBrightness = DxvaImports.GetMonitorBrightness(monitors[0].hPhysicalMonitor, ref minValue,
                ref currentBrightness, ref maxValue);

            var hwValue = (int)((maxValue - minValue) * newValue / 100 + minValue);

            _logger.Information(
                $"[SetBrightness:Win] '{monitor.ModelName}': DXVA min={minValue}, current={currentBrightness}, max={maxValue}, converted hwValue={hwValue}");

            if (!getMonBrightness)
            {
                var lastWin32Error = Marshal.GetLastWin32Error();
                var ex = new Win32Exception(lastWin32Error);
                if (ex.HResult != -2147467259)
                {
                    _logger.Error(
                        $"[SetBrightness:Win] {nameof(DxvaImports.GetMonitorBrightness)} for '{monitor.ModelName}' (dev={monitor.DeviceName}) ({lastWin32Error}) Error: 0x{ex.HResult:X} {ex.Message} - {ex.HResult}");
                    _logger.Error($"[SetBrightness:Win] Returned brightness: {currentBrightness}");
                }
            }

            if (currentBrightness == (uint)hwValue)
            {
                _logger.Information($"[SetBrightness:Win] '{monitor.ModelName}': skipped — already at {hwValue}");
                return true;
            }

            var result = DxvaImports.SetMonitorBrightness(monitors[0].hPhysicalMonitor, (uint)hwValue);
            _logger.Information(
                $"[SetBrightness:Win] '{monitor.ModelName}': SetMonitorBrightness from {currentBrightness} to {hwValue}, success={result}");
            return result;
        }

        private List<MonitorHandleInfo> GetMonitorHandles()
        {
            var col = new List<MonitorHandleInfo>();

            bool ResultCallback(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData)
            {
                var mi = new MonitorInfoEx();
                mi.Size = Marshal.SizeOf(mi);

                var success = WindowsMonitorApiImports.GetMonitorInfo(hMonitor, ref mi);
                if (!success)
                {
                    //TODO Error Handling
                    var err = Marshal.GetLastWin32Error();
                    _logger.Error($"GetMonitorInfo failed: {err}");
                    return false;
                }

                var dev = new DISPLAY_DEVICE();
                dev.cb = Marshal.SizeOf(dev);
                if (!WindowsMonitorApiImports.EnumDisplayDevices(mi.DeviceName, 0, ref dev, 1))
                {
                    //TODO Error Handling
                    var err = Marshal.GetLastWin32Error();
                    _logger.Error($"EnumDisplayDevices failed: {err}");
                }

                var di = new MonitorHandleInfo(mi.DeviceName, hMonitor);
                col.Add(di);
                return true;
            }

            WindowsMonitorApiImports.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, ResultCallback, IntPtr.Zero);

            return col;
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
