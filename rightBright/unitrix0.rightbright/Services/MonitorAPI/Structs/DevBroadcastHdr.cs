using System;
using System.Runtime.InteropServices;

namespace unitrix0.rightbright.Services.MonitorAPI.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DevBroadcastHdr
    {
        internal UInt32 Size;
        internal UInt32 DeviceType;
        internal UInt32 Reserved;
    }
}