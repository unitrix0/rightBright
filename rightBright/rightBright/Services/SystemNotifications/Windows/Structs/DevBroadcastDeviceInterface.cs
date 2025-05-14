using System;
using System.Runtime.InteropServices;

namespace rightBright.Services.SystemNotifications.Windows.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DevBroadcastDeviceInterface
    {
        internal int Size;
        internal int DeviceType;
        internal int Reserved;
        internal Guid ClassGuid;
        internal short Name;
    }
}
