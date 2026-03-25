using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using rightBright.Brightness;
using rightBright.Brightness.Calculators;
using rightBright.Services.Brightness;
using rightBright.Services.LoadingState;
using rightBright.Services.Monitors;
using rightBright.Services.Monitors.Enummerators;
using rightBright.Services.Sensors;
using rightBright.Services.SystemNotifications;
using rightBright.Services.SystemNotifications.Linux;
using rightBright.Services.SystemNotifications.Windows;
using rightBright.Settings;
using rightBright.ViewModels;
using rightBright.Views;
using Serilog;

namespace rightBright;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = InitializeDependencyInjection();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var appViewModel = services.GetRequiredService<ApplicationViewModel>();
            appViewModel.OnOpenMainWindow += () =>
            {
                var viewModel = services.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = viewModel
                };

                desktop.MainWindow.Show();
            };

            DataContext = appViewModel;
            var brightnessController = services.GetRequiredService<IBrightnessController>();
            brightnessController.Run();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider InitializeDependencyInjection()
    {
        // If you use CommunityToolkit, the line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        // Serilog file logging (with rolling file rotation) for runtime debugging.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "rightbright-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Debug()
            .CreateLogger();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<CurveEditorViewModel>();
        serviceCollection.AddSingleton<ISettings>(_ => AppSettings.Load());
        serviceCollection.AddSingleton<ILoadingMonitorStateService, LoadingMonitorStateService>();
        serviceCollection.AddSingleton<IBrightnessController, BrightnessController>();
        serviceCollection.AddSingleton<ApplicationViewModel>();

        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<ISetBrightnessService>(servies =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new SetBrightnessServiceLinux(servies.GetRequiredService<Serilog.ILogger>())
                : new SetBrightnessServiceWin(servies.GetRequiredService<Serilog.ILogger>()));

        serviceCollection.AddSingleton<IMonitorChangedNotificationService>(_ =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new LinuxMonitorChangedNotificationService()
                : new WinMonitorChangedNotificationService());

        serviceCollection.AddSingleton<IPowerNotificationService>(_ =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new LinuxPowerNotificationService()
                : new WinPowerNotificationService());

        serviceCollection.AddSingleton<IBrightnessCalculator, BezierBrightnessCalculator>();
        serviceCollection.AddSingleton<ISensorRepo, SensorRepo>();
        serviceCollection.AddSingleton<ISensorService, YoctoSensorService>();
        serviceCollection.AddSingleton<Serilog.ILogger>(Log.Logger);
        serviceCollection.AddSingleton<IMonitorEnummerationService>(services =>
        {
            var logger = services.GetRequiredService<Serilog.ILogger>();
            var changeNotificationService = services.GetRequiredService<IMonitorChangedNotificationService>();
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new LinuxMonitorEnumService(logger, changeNotificationService)
                : new WinMonitorEnumService(logger, changeNotificationService);
        });

        serviceCollection.AddSingleton<ContentViewFactory>();
        serviceCollection.AddScoped<Func<Type, MainWindowContentViewModel>>(services => requestedType =>
            requestedType switch
            {
                _ when requestedType == typeof(CurveEditorViewModel) => services.GetRequiredService<CurveEditorViewModel>(),
                _ => throw new InvalidOperationException($"Page of type {requestedType.FullName} has no view model")
            });

        var services = serviceCollection.BuildServiceProvider();
        return services;
    }

    /// <summary>
    /// Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
    /// More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
    /// </summary>
    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
