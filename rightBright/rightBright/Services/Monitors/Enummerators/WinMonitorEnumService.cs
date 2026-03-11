using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using rightBright.Services.Logging;
using rightBright.Services.SystemNotifications;
using rightBright.WindowsApi.Monitor;
using rightBright.WindowsApi.Monitor.Structs;

namespace rightBright.Services.Monitors
{
    public class WinMonitorEnumService : IMonitorEnummerationService
    {
        private readonly ILoggingService _logger;
        private readonly IMonitorChangedNotificationService _monitorChangedNotificationService;
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
        private readonly List<DisplayInfo> _displays = [];

        public WinMonitorEnumService(ILoggingService logger, IMonitorChangedNotificationService monitorChangedNotificationService)
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
                            _logger.WriteError($"Failed to get monitor info: {err}");
                            return false;
                        }

                        var dev = new DISPLAY_DEVICE();
                        dev.cb = Marshal.SizeOf(dev);
                        if (!WindowsMonitorApiImports.EnumDisplayDevices(mi.DeviceName, 0, ref dev, 1))
                        {
                            var err = Marshal.GetLastWin32Error();
                            _logger.WriteError($"Failed to enumerate display device: {err}");
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
                            _logger.WriteError($"Failed to get monitor info during update: {err}");
                            return false;
                        }

                        var dev = new DISPLAY_DEVICE();
                        dev.cb = Marshal.SizeOf(dev);
                        if (!WindowsMonitorApiImports.EnumDisplayDevices(mi.DeviceName, 0, ref dev, 1))
                        {
                            var err = Marshal.GetLastWin32Error();
                            _logger.WriteError($"Failed to enumerate display device during update: {err}");
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
                _logger.WriteError($"Error updating displays cache: {ex.Message}");
            }
            finally
            {
                _cacheLock.Release();
            }
        }
    }
}
