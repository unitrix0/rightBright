using Prism.Ioc;
using Prism.Unity;
using System;
using System.Configuration;
using System.Windows;
using Prism.Mvvm;
using unitrix0.rightbright.Brightness;
using unitrix0.rightbright.Brightness.Calculators;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Services.Brightness;
using unitrix0.rightbright.Services.CurveCalculation;
using unitrix0.rightbright.Services.MonitorAPI;
using unitrix0.rightbright.Settings;
using unitrix0.rightbright.Windows;
using unitrix0.rightbright.Windows.ViewModel;

namespace unitrix0.rightbright
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var x = new BrightnessController(Container.Resolve<ISensorService>(),
                Container.Resolve<ISetBrightnessService>(),
                Container.Resolve<IMonitorService>(),
                Container.Resolve<IBrightnessCalculator>());

            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.RegisterSingleton<IMonitorEnummerationService, MonitorEnummerationService>();
            containerRegistry.RegisterSingleton<IMonitorService, MonitorService>();
            containerRegistry.RegisterSingleton<IBrightnessCalculator, ProgressiveBrightnessCalculator>();
            containerRegistry.RegisterSingleton<ISensorService, SensorService>();
            containerRegistry.RegisterSingleton<ISetBrightnessService, SetBrightnessService>();
            containerRegistry.RegisterSingleton<ISettings>(Settings.Settings.Load);
            containerRegistry.RegisterSingleton<ICurveCalculationService, CurveCalculationService>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var settings = Container.Resolve<ISettings>();
            settings.Save();
        }
    }
}
