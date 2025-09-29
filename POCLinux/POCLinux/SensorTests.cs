using System.Timers;
using Timer = System.Timers.Timer;

public static class SensorTests
{
    public static void Run()
    {
        Timer handleYapiEventsTimer = new();
        var error = "";
        bool init = false;

        if (YAPI.RegisterHub("usb", ref error) == YAPI.SUCCESS)
        {
            handleYapiEventsTimer.Interval = 2000;
            handleYapiEventsTimer.Elapsed += HandleYapiEventsTimerOnElapsed;

            var sensor = YLightSensor.FindLightSensor("LIGHTMK3-17AE3E.lightSensor");
            sensor.clearCache();
            sensor.registerTimedReportCallback(TimedReport);
            handleYapiEventsTimer.Start();

            do
            {
                if (sensor.isOnline())
                {
                    if (!init)
                    {
                        sensor.stopDataLogger();
                        sensor.set_logFrequency("OFF");
                        init = true;
                    }
                    // Console.WriteLine($"\r{DateTime.Now.ToShortTimeString()} - {sensor.get_currentValue()}");
                }
                else
                {
                    Console.WriteLine("\rOffline\t\t\t");
                }

                Thread.Sleep(1000);
            } while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q));
        }
        else
        {
            Console.WriteLine(error);
        }
    }

    static void TimedReport(YLightSensor func, YMeasure measure)
    {
        var currentValue = func.get_currentValue();
        Console.WriteLine($"\r{DateTime.Now.ToShortTimeString()} - {currentValue} - timer");
    }

    static void HandleYapiEventsTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        string errmsg = "";
        YAPI.HandleEvents(ref errmsg);
    }
}
