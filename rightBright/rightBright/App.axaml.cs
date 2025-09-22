using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using rightBright.Services.Logging;
using rightBright.Services.Monitors;
using rightBright.Services.Sensors;
using rightBright.Settings;
using rightBright.ViewModels;
using rightBright.Views;
using Tmds.DBus.Protocol;
using Address = Tmds.DBus.Address;

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
            var viewModel = services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider InitializeDependencyInjection()
    {
        // If you use CommunityToolkit, the line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<MainWindowViewModel>();
        serviceCollection.AddSingleton<ISettings, AppSettings>();
        serviceCollection.AddSingleton<ISensorRepo, SensorRepo>();
        serviceCollection.AddSingleton<ILoggingService, LoggingService>();
        serviceCollection.AddSingleton<ISensorService, YoctoSensorService>();
        serviceCollection.AddSingleton<IMonitorEnummerationService>(_ =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new LinuxMonitorEnumService()
                : new WinMonitorEnumService());

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
