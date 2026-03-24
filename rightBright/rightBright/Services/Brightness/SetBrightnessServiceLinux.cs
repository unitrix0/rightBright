using System;
using System.Threading.Tasks;
using rightBright.Services.DBus.ddcutil;
using rightBright.Services.Logging;
using Address = Tmds.DBus.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace rightBright.Services.Brightness;

public class SetBrightnessServiceLinux : ISetBrightnessService
{
    private readonly Connection _dbusConnection;
    private readonly ILoggingService _logger;

    public SetBrightnessServiceLinux(ILoggingService logger)
    {
        _logger = logger;
        _dbusConnection = new Connection(Address.Session);
    }

    public async Task<bool> SetBrightness(DisplayInfo monitor, int newValue)
    {
        try
        {
            _logger.WriteInformation($"[SetBrightness:Linux] '{monitor.ModelName}' (dev={monitor.DeviceName}): requested={newValue}%");

            const int minValue = 1;
            await _dbusConnection.ConnectAsync();
            var ddcUtilService = new DdcutilService(_dbusConnection, "com.ddcutil.DdcutilService");
            var ddcUtil = ddcUtilService.CreateDdcutilInterface("/com/ddcutil/DdcutilObject");
            var displayNumber = Convert.ToInt32(monitor.DeviceName);
            
            var currentVcp = await ddcUtil.GetVcpAsync(displayNumber, "", 16, 0x0);
            var maxValue = currentVcp.VcpMaxValue;
            var hwValue = (maxValue - minValue) * newValue / 100 + minValue;

            _logger.WriteInformation(
                $"[SetBrightness:Linux] '{monitor.ModelName}': VCP current={currentVcp.VcpCurrentValue}, max={maxValue}, converted hwValue={hwValue}");

            if (currentVcp.VcpCurrentValue == hwValue)
            {
                _logger.WriteInformation($"[SetBrightness:Linux] '{monitor.ModelName}': skipped — already at {hwValue}");
                return true;
            }

            var result = await ddcUtil.SetVcpAsync(displayNumber, "", 16, (ushort)hwValue, 0x0);
            _logger.WriteInformation(
                $"[SetBrightness:Linux] '{monitor.ModelName}': SetVcp to {hwValue}, ErrorStatus={result.ErrorStatus}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.WriteError($"[SetBrightness:Linux] '{monitor.ModelName}' (dev={monitor.DeviceName}): {ex.Message}\n{ex.StackTrace}");
            return false; 
        }
    }
}
