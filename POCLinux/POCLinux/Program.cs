// See https://aka.ms/new-console-template for more information


using System.Text.Json;
using System.Text.Json.Serialization;
using NetworkManager.DBus;
using POCLinux.dbus.ddcutil;
using ScreenBrightness.DBus;
using Tmds.DBus;
using Connection = Tmds.DBus.Protocol.Connection;

var dbusConnection = new Connection(Address.Session);
await dbusConnection.ConnectAsync();

// await WatchAddDisplay(dbusConnection);
await DdcUtilSetBrightness(dbusConnection);

SensorTests.Run();

async Task WatchAddDisplay(Connection connection)
{
    var screenBrightnessSvc = new ScreenBrightnessService(connection, "org.kde.ScreenBrightness");
    var screenBrightness = screenBrightnessSvc.CreateScreenBrightness("/org/kde/ScreenBrightness");

    var names = await screenBrightness.GetDisplaysDBusNamesAsync();
    foreach (string name in names)
    {
        Console.WriteLine(name);
    }

    await screenBrightness.WatchDisplayAddedAsync((exception, s) => { Console.WriteLine($"Added: {s}"); });
    await screenBrightness.WatchDisplayRemovedAsync((exception, s) => { Console.WriteLine($"Removed: {s}"); });

    do
    {
        // Console.Write($"\r{DateTime.Now}");
        // System.Threading.Thread.Sleep(500);
        await Task.Delay(50);
    } while (Console.KeyAvailable == false);
}

async Task DdcUtilSetBrightness(Connection dbusConnection1)
{
    var ddcutilSvc = new DdcutilService(dbusConnection1, DdcutilService.BusName);
    var ddcutil = ddcutilSvc.CreateDdcutilInterface("/com/ddcutil/DdcutilObject");
    
    var detected = await ddcutil.ListDetectedAsync(0x0);
    
    var result = await ddcutil.DetectAsync(0x0);
    var attributes = await ddcutil.GetAttributesReturnedByDetectAsync();
    var resultSet = await ddcutil.SetVcpAsync(2, "", 0x10, 38, 0x0);
    // await ddcutil.SetVcpAsync(2, "", 0x10, 52, 0x0);
    Console.WriteLine("done");
    
}
