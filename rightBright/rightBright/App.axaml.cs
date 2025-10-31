using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using rightBright.Brightness;
using rightBright.Brightness.Calculators;
using rightBright.Services.Brightness;
using rightBright.Services.CurveCalculation;
using rightBright.Services.DBus.ddcutil;
using rightBright.Services.Logging;
using rightBright.Services.Monitors;
using rightBright.Services.Sensors;
using rightBright.Services.SystemNotifications;
using rightBright.Services.SystemNotifications.Linux;
using rightBright.Services.SystemNotifications.Windows;
using rightBright.Settings;
using rightBright.ViewModels;
using rightBright.Views;
using Tmds.DBus.Protocol;
using unitrix0.rightbright.Brightness.Calculators;
using Address = Tmds.DBus.Address;
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
        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<ApplicationViewModel>();
        serviceCollection.AddSingleton<CurveEditorViewModel>();

        serviceCollection.AddSingleton<ISettings>(_ => AppSettings.Load());
        serviceCollection.AddSingleton<IBrightnessController, BrightnessController>();
        serviceCollection.AddSingleton<ISetBrightnessService>(servies =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new SetBrightnessServiceLinux()
                : new SetBrightnessServiceWin(servies.GetRequiredService<ILoggingService>(),
                    servies.GetRequiredService<IMonitorEnummerationService>()));

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
        serviceCollection.AddSingleton<IMonitorEnummerationService>(_ =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new LinuxMonitorEnumService()
                : new WinMonitorEnumService());

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
