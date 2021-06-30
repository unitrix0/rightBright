using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using unitrix0.rightbright.Services.MonitorAPI.Structs;

namespace unitrix0.rightbright.Services.Brightness
{
    public class SetBrightnessService : ISetBrightnessService
    {
        
        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);
        

        private uint _minValue;
        private uint _maxValue;
        private uint _currentValue;

        public uint CurrentBrightness => _currentValue;


        public void SetBrightness(IntPtr monitorHandle, int newValue)
        {
            newValue = newValue > 100 ? 100 : newValue;
            newValue = newValue < 1 ? 1 : newValue;
            _currentValue = (_maxValue - _minValue) * (uint)newValue / 100u + _minValue;
            var monitors = GetPhysicalMonitors(monitorHandle);

            var getMonBrightness = GetMonitorBrightness(monitors[0].hPhysicalMonitor, ref _minValue, ref _currentValue, ref _maxValue);
            if (!getMonBrightness)
            {
                var lastWin32Error = Marshal.GetLastWin32Error();
                var ex = new Win32Exception(lastWin32Error);
                Debug.Print($"\t\t *** {nameof(GetMonitorBrightness)} ({lastWin32Error}) Error: 0x{ex.HResult:X} {ex.Message}");
            }
            var result = SetMonitorBrightness(monitors[0].hPhysicalMonitor, (uint)newValue);
            Debug.Print($"{nameof(SetBrightness)} FROM:{_currentValue} TO: {newValue} => success: {result}");
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