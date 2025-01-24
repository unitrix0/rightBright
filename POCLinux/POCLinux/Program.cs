// See https://aka.ms/new-console-template for more information


using System;
using System.Text;

var error = "";
if (YAPI.RegisterHub("usb", ref error) == YAPI.SUCCESS)
{
    var sensor =  YLightSensor.FindLightSensor("LIGHTMK3-17AE3E.lightSensor");
    do
    {
        if (sensor.isOnline())
        {
            Console.WriteLine($"\r{DateTime.Now.ToShortTimeString()} - {sensor.get_currentValue()}");
        }
        else
        {
            Console.WriteLine("\rOffline\t\t\t");
        }
        
        Thread.Sleep(1000);
    } while (!(Console.KeyAvailable&& Console.ReadKey(true).Key == ConsoleKey.Q));
}
else
{
    Console.WriteLine(error);
}
