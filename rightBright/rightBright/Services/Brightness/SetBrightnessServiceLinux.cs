using System;
using rightBright.Services.DBus.ddcutil;
using Address = Tmds.DBus.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace rightBright.Services.Brightness;

public class SetBrightnessServiceLinux : ISetBrightnessService
{
    private readonly Connection _dbusConnection;
    private int _maxValue;
    private int _minValue;

    public SetBrightnessServiceLinux()
    {
        _dbusConnection = new Connection(Address.Session);
    }

    public void SetBrightness(DisplayInfo monitor, int newValue)
    {
        _minValue = 0; //DDCutil does not return a minimum value. So we asume alaways 0
        _dbusConnection.ConnectAsync().GetAwaiter();
        var ddcUtilService = new DdcutilService(_dbusConnection, "com.ddcutil.DdcutilService");
        var ddcUtil = ddcUtilService.CreateDdcutilInterface("/com/ddcutil/DdcutilObject");

        var displayNumber = Convert.ToInt32(monitor.DeviceName);
        var currentVcp = ddcUtil.GetVcpAsync(displayNumber, "", 16, 0x0)
            .GetAwaiter()
            .GetResult();

        _maxValue = currentVcp.VcpMaxValue;
        var currentBrightness = (_maxValue - _minValue) * (uint)newValue / 100u + _minValue;

        if (currentBrightness == newValue) return;
        ddcUtil.SetVcpAsync(displayNumber, "", 16, (ushort)newValue, 0x0).GetAwaiter();
    }
}
