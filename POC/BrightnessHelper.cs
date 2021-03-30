using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SetBrightness
{
    public class BrightnessHelper : IDisposable
    {
        [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
        public static extern IntPtr MonitorFromWindow([In] IntPtr hwnd, uint dwFlags);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, ref PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

        private PHYSICAL_MONITOR[] _physicalMonitorArray;

        private readonly IntPtr _firstMonitorHandle;

        private readonly uint _minValue = 0;
        private readonly uint _maxValue = 0;
        private uint _currentValue = 0;

        public uint CurrentBrightness => _currentValue;

        public BrightnessHelper()
        {
        }

        public BrightnessHelper(IntPtr windowHandle)
        {
            uint dwFlags = 0u;
            IntPtr ptr = MonitorFromWindow(windowHandle, dwFlags);

            _physicalMonitorArray = GetPhysicalMonitors(ptr);
            _firstMonitorHandle = _physicalMonitorArray[0].hPhysicalMonitor;
            if (!GetMonitorBrightness(_firstMonitorHandle, ref _minValue, ref _currentValue, ref _maxValue))
            {
                throw new Exception("Cannot get monitor brightness!");
            }
        }

        public void SetBrightness(int newValue) // 0 ~ 100
        {
            newValue = Math.Min(newValue, Math.Max(0, newValue));
            _currentValue = (_maxValue - _minValue) * (uint)newValue / 100u + _minValue;
            SetMonitorBrightness(_firstMonitorHandle, _currentValue);
        }

        public void SetBrightness(IntPtr monitorHandle, int newValue)
        {
            newValue = Math.Min(newValue, Math.Max(0, newValue));
            _currentValue = (_maxValue - _minValue) * (uint)newValue / 100u + _minValue;
            var monitors = GetPhysicalMonitors(monitorHandle);

            var result = SetMonitorBrightness(monitors[0].hPhysicalMonitor, (uint)newValue);
            Debug.Print($"{nameof(SetBrightness)} success: {result}");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (_physicalMonitorArray.Length > 0)
            {
                DestroyPhysicalMonitors((uint)_physicalMonitorArray.Length, ref _physicalMonitorArray);
            }
        }

        private PHYSICAL_MONITOR[] GetPhysicalMonitors(IntPtr ptr)
        {
            var physicalMonitorCount = GetPhysicalMonitorCount(ptr);
            var monitors = new PHYSICAL_MONITOR[physicalMonitorCount];
            if (!GetPhysicalMonitorsFromHMONITOR(ptr, physicalMonitorCount, monitors))
            {
                throw new Exception("Cannot get phisical monitor handle!");
            }

            return monitors;
        }

        private uint GetPhysicalMonitorCount(IntPtr ptr)
        {
            uint count = 0;
            if (!GetNumberOfPhysicalMonitorsFromHMONITOR(ptr, ref count))
            {
                var error = Marshal.GetLastWin32Error();
                throw new Exception($"Cannot get monitor count! ({error})");
            }

            return count;
        }
    }
}