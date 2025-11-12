using System;
using System.Threading.Tasks;
using rightBright.Services.DBus.ddcutil;
using Address = Tmds.DBus.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace rightBright.Services.Brightness;

public class SetBrightnessServiceLinux : ISetBrightnessService
{
    private readonly Connection _dbusConnection;

    public SetBrightnessServiceLinux()
    {
        _dbusConnection = new Connection(Address.Session);
    }

    public async Task SetBrightness(DisplayInfo monitor, int newValue)
    {
        try
        {
            const int minValue = 1; //DDCutil does not return a minimum value. So we asume alaways 1
            await _dbusConnection.ConnectAsync();
            var ddcUtilService = new DdcutilService(_dbusConnection, "com.ddcutil.DdcutilService");
            var ddcUtil = ddcUtilService.CreateDdcutilInterface("/com/ddcutil/DdcutilObject");
            var displayNumber = Convert.ToInt32(monitor.DeviceName);
            
            var currentVcp = await ddcUtil.GetVcpAsync(displayNumber, "", 16, 0x0);
            var maxValue = currentVcp.VcpMaxValue;
            newValue = (maxValue - minValue) * newValue / 100 + minValue;

            if (currentVcp.VcpCurrentValue == newValue) return;
            ddcUtil.SetVcpAsync(displayNumber, "", 16, (ushort)newValue, 0x0).GetAwaiter();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
