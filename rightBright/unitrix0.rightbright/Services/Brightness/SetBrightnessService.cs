﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using unitrix0.rightbright.Services.MonitorAPI.Structs;

namespace unitrix0.rightbright.Services.Brightness
{
    public class SetBrightnessService : IDisposable, ISetBrightnessService
    {
        [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
        private static extern IntPtr MonitorFromWindow([In] IntPtr hwnd, uint dwFlags);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, ref PHYSICAL_MONITOR[] pPhysicalMonitorArray);

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

        private PHYSICAL_MONITOR[] _physicalMonitorArray;

        private uint _minValue = 0;
        private uint _maxValue = 0;
        private uint _currentValue = 0;

        public uint CurrentBrightness => _currentValue;

        public SetBrightnessService()
        {
        }

        public void SetBrightness(IntPtr monitorHandle, int newValue)
        {
            newValue = Math.Min(newValue, Math.Max(0, newValue));
            //_currentValue = (_maxValue - _minValue) * (uint)newValue / 100u + _minValue;
            //if (!GetMonitorBrightness(monitorHandle, ref _minValue, ref _currentValue, ref _maxValue))
            //{
            //    var ex = new Win32Exception(Marshal.GetLastWin32Error());
            //    Debug.Print($"{nameof(GetMonitorBrightness)} Error: {ex.HResult} {ex.Message}");
            //}

            var monitors = GetPhysicalMonitors(monitorHandle);
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
    }
}