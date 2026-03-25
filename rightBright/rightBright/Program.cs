using System;
using Avalonia;
using rightBright.Updates;

namespace rightBright;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if (OperatingSystem.IsWindows())
        {
            // Best-effort automated updates; never block UI startup.
            try
            {
                UpdateManager.CheckAndApplyUpdatesAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch
            {
                // Ignore update failures and continue starting the app.
            }
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}