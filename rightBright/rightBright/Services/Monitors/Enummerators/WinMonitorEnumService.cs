using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using rightBright.Services.SystemNotifications;
using rightBright.WindowsApi.Monitor;
using rightBright.WindowsApi.Monitor.Structs;

namespace rightBright.Services.Monitors
{
    public class WinMonitorEnumService : IMonitorEnummerationService
    {
        private readonly ILogger _logger;
        private readonly IMonitorChangedNotificationService _monitorChangedNotificationService;
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
        private readonly List<DisplayInfo> _displays = [];

        public WinMonitorEnumService(ILogger logger, IMonitorChangedNotificationService monitorChangedNotificationService)
        {
            _logger = logger;
            _monitorChangedNotificationService = monitorChangedNotificationService;
            _monitorChangedNotificationService.DeviceChangedMessage += (_, _) => _ = Update();
        }

        /// <summary>
        /// Returns informations about the connect Mointors
        /// </summary>
        /// <returns>collection of Display Info</returns>
        public async Task<List<DisplayInfo>> GetDisplays()
        {
            await _cacheLock.WaitAsync();
            try
            {
                if (_displays.Count > 0) return _displays;

                return await Task.Factory.StartNew(() =>
                {
                    WindowsMonitorApiImports.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, ResultCallback, IntPtr.Zero);
                    return _displays;

                    bool ResultCallback(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData)
                    {
                        var mi = new MonitorInfoEx();
                        mi.Size = Marshal.SizeOf(mi);

                        var success = WindowsMonitorApiImports.GetMonitorInfo(hMonitor, ref mi);
                        if (!success)
                        {
                            var err = Marshal.GetLastWin32Error();
                            _logger.Error(
                                $"Failed to get monitor info: {DisplayDeviceEnumerationDiagnostics.FormatGetMonitorInfoFailure(hMonitor, err)}");
                            return false;
                        }

                        const uint enumDisplayDevicesFlags = 1; // EDD_GET_DEVICE_INTERFACE_NAME
                        var dev = new DISPLAY_DEVICE();
                        dev.cb = Marshal.SizeOf(dev);
                        if (!WindowsMonitorApiImports.EnumDisplayDevices(mi.DeviceName, 0, ref dev, enumDisplayDevicesFlags))
                        {
                            var err = Marshal.GetLastWin32Error();
                            _logger.Error(
                                $"Failed to enumerate display device: {DisplayDeviceEnumerationDiagnostics.FormatEnumDisplayDevicesFailure(mi.DeviceName, dev, enumDisplayDevicesFlags, err)}");
                        }

                        var di = new DisplayInfo
                        {
                            ScreenWidth = mi.Monitor.Right - mi.Monitor.Left,
                            ScreenHeight = mi.Monitor.Bottom - mi.Monitor.Top,
                            MonitorArea = mi.Monitor,
                            WorkArea = mi.WorkArea,
                            IsPrimaryMonitor = Convert.ToBoolean(mi.Flags),
                            ModelName = dev.DeviceString,
                            DeviceName = mi.DeviceName
                        };

                        Debug.Print($"Display Found: {di.ModelName}");
                        _displays.Add(di);
                        return true;
                    }
                });
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        private async Task Update()
        {
            await _cacheLock.WaitAsync();
            try
            {
                _displays.Clear();
                
                // Repopulate the cache
                await Task.Factory.StartNew(() =>
                {
                    WindowsMonitorApiImports.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, ResultCallback, IntPtr.Zero);
                    
                    bool ResultCallback(IntPtr hMonitor, IntPtr hdcMonitor, ref RectStruct lprcMonitor, IntPtr dwData)
                    {
                        var mi = new MonitorInfoEx();
                        mi.Size = Marshal.SizeOf(mi);

                        var success = WindowsMonitorApiImports.GetMonitorInfo(hMonitor, ref mi);
                        if (!success)
                        {
                            var err = Marshal.GetLastWin32Error();
                            _logger.Error(
                                $"Failed to get monitor info during update: {DisplayDeviceEnumerationDiagnostics.FormatGetMonitorInfoFailure(hMonitor, err)}");
                            return false;
                        }

                        const uint enumDisplayDevicesFlags = 1; // EDD_GET_DEVICE_INTERFACE_NAME
                        var dev = new DISPLAY_DEVICE();
                        dev.cb = Marshal.SizeOf(dev);
                        if (!WindowsMonitorApiImports.EnumDisplayDevices(mi.DeviceName, 0, ref dev, enumDisplayDevicesFlags))
                        {
                            var err = Marshal.GetLastWin32Error();
                            _logger.Error(
                                $"Failed to enumerate display device during update: {DisplayDeviceEnumerationDiagnostics.FormatEnumDisplayDevicesFailure(mi.DeviceName, dev, enumDisplayDevicesFlags, err)}");
                        }

                        var di = new DisplayInfo
                        {
                            ScreenWidth = mi.Monitor.Right - mi.Monitor.Left,
                            ScreenHeight = mi.Monitor.Bottom - mi.Monitor.Top,
                            MonitorArea = mi.Monitor,
                            WorkArea = mi.WorkArea,
                            IsPrimaryMonitor = Convert.ToBoolean(mi.Flags),
                            ModelName = dev.DeviceString,
                            DeviceName = mi.DeviceName
                        };

                        Debug.Print($"Display Found: {di.ModelName}");
                        _displays.Add(di);
                        return true;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating displays cache: {ex.Message}");
            }
            finally
            {
                _cacheLock.Release();
            }
        }
    }
}
