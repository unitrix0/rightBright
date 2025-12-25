using System;
using System.Linq;
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
using rightBright.Services.CurveCalculation;
using rightBright.Services.Logging;
using rightBright.Services.Monitors;
using rightBright.Services.Monitors.Enummerators;
using rightBright.Services.Sensors;
using rightBright.Services.SystemNotifications;
using rightBright.Services.SystemNotifications.Linux;
using rightBright.Services.SystemNotifications.Windows;
using rightBright.Settings;
using rightBright.ViewModels;
using rightBright.Views;
using ApplicationViewModel = rightBright.ViewModels.ApplicationViewModel;

namespace rightBright;

public partial class App : Application
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

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<CurveEditorViewModel>();

        serviceCollection.AddSingleton<ISettings>(_ => AppSettings.Load());
        
        // Register ApplicationViewModel first (without IBrightnessController to avoid circular dependency)
        serviceCollection.AddSingleton<ApplicationViewModel>(_ => new ApplicationViewModel());
        
        // Register BrightnessController with ApplicationViewModel
        serviceCollection.AddSingleton<IBrightnessController>(services =>
        {
            var sensorService = services.GetRequiredService<ISensorService>();
            var brightnessService = services.GetRequiredService<ISetBrightnessService>();
            var monitorService = services.GetRequiredService<IMonitorEnummerationService>();
            var brightnessCalculator = services.GetRequiredService<IBrightnessCalculator>();
            var settings = services.GetRequiredService<ISettings>();
            var monitorNotificationService = services.GetRequiredService<IMonitorChangedNotificationService>();
            var powerNotificationService = services.GetRequiredService<IPowerNotificationService>();
            var logger = services.GetRequiredService<ILoggingService>();
            var applicationViewModel = services.GetRequiredService<ApplicationViewModel>();
            
            var brightnessController = new BrightnessController(
                sensorService,
                brightnessService,
                monitorService,
                brightnessCalculator,
                settings,
                monitorNotificationService,
                powerNotificationService,
                logger,
                applicationViewModel);
            
            // Set the brightness controller on ApplicationViewModel after creation
            applicationViewModel.SetBrightnessController(brightnessController);
            
            return brightnessController;
        });
        
        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<ISetBrightnessService>(servies =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new SetBrightnessServiceLinux()
                : new SetBrightnessServiceWin(servies.GetRequiredService<ILoggingService>()));

        serviceCollection.AddSingleton<IMonitorChangedNotificationService>(services =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new LinuxMonitorChangedNotificationService()
                : new WinMonitorChangedNotificationService());

        serviceCollection.AddSingleton<IPowerNotificationService>(services =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new LinuxPowerNotificationService()
                : new WinPowerNotificationService());

        serviceCollection.AddSingleton<IBrightnessCalculator, ProgressiveBrightnessCalculator>();
        serviceCollection.AddSingleton<ISensorRepo, SensorRepo>();
        serviceCollection.AddSingleton<ILoggingService, LoggingService>();
        serviceCollection.AddSingleton<ISensorService, YoctoSensorService>();
        serviceCollection.AddSingleton<ICurveCalculationService, CurveCalculationService>();
        serviceCollection.AddSingleton<IMonitorEnummerationService>(services =>
        {
            var logger = services.GetRequiredService<ILoggingService>();
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
