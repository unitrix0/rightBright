using System;
using Avalonia;
using Microsoft.Win32;
using Velopack;

namespace rightBright;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        VelopackApp.Build()
            .OnAfterInstallFastCallback((v) =>
            {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", true);
                key?.SetValue("rightBright",
                    $"\"{Environment.ProcessPath}\"");
            })
            .OnBeforeUninstallFastCallback((v) =>
            {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", true);
                key?.DeleteValue("rightBright", throwOnMissingValue: false);
            })
            .Run();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}