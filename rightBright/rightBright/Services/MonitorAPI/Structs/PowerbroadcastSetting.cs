using System;
using System.Runtime.InteropServices;

namespace rightBright.Services.MonitorAPI.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct PowerbroadcastSetting
    {
        public Guid PowerSetting;

        public uint DataLength;

        public byte Data;
    };
}
