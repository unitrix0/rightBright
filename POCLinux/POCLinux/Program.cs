// See https://aka.ms/new-console-template for more information


using System.Timers;
using POCLinux.dbus.ddcutil;
using Tmds.DBus;
using Connection = Tmds.DBus.Protocol.Connection;
using Timer = System.Timers.Timer;

var dbusConnection = new Connection(Address.Session);
await dbusConnection.ConnectAsync();

var ddcutilSvc = new DdcutilService(dbusConnection, DdcutilService.BusName);
var ddcutil = ddcutilSvc.CreateDdcutilInterface("/com/ddcutil/DdcutilObject");

(int NumberOfDisplays, (int, int, int, string, string, string, ushort, string, uint)[] DetectedDisplays, int ErrorStatus, string ErrorMessage) result = await ddcutil.DetectAsync(0x0);
var attributes = await ddcutil.GetAttributesReturnedByDetectAsync();
Console.ReadKey();
// await ddcutil.SetVcpAsync(2, "", 0x10, 38, 0x0);
// await ddcutil.SetVcpAsync(2, "", 0x10, 52, 0x0);

// Timer handleYapiEventsTimer = new();
// var error = "";
// bool init= false;
//
// if (YAPI.RegisterHub("usb", ref error) == YAPI.SUCCESS)
// {
//     handleYapiEventsTimer.Interval = 2000;
//     handleYapiEventsTimer.Elapsed += HandleYapiEventsTimerOnElapsed;
//     
//     var sensor =  YLightSensor.FindLightSensor("LIGHTMK3-17AE3E.lightSensor");
//     sensor.clearCache();
//     sensor.registerTimedReportCallback(TimedReport);
//     handleYapiEventsTimer.Start();
//     
//     do
//     {
//         if (sensor.isOnline())
//         {
//             
//             if (!init)
//             {
//                 sensor.stopDataLogger();
//                 sensor.set_logFrequency("OFF");
//                 init = true;
//             }
//             // Console.WriteLine($"\r{DateTime.Now.ToShortTimeString()} - {sensor.get_currentValue()}");
//         }
//         else
//         {
//             Console.WriteLine("\rOffline\t\t\t");
//         }
//         
//         Thread.Sleep(1000);
//     } while (!(Console.KeyAvailable&& Console.ReadKey(true).Key == ConsoleKey.Q));
// }
// else
// {
//     Console.WriteLine(error);
// }
//
// void TimedReport(YLightSensor func, YMeasure measure)
// {
//     var currentValue = func.get_currentValue();
//     Console.WriteLine($"\r{DateTime.Now.ToShortTimeString()} - {currentValue} - timer");
//     
// }
//
// void HandleYapiEventsTimerOnElapsed(object? sender, ElapsedEventArgs e)
// {
//     string errmsg = "";
//     YAPI.HandleEvents(ref errmsg);
// }
