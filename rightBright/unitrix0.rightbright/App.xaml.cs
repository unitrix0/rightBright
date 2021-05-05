using Prism.Ioc;
using Prism.Unity;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Prism.Commands;
using Prism.Mvvm;
using unitrix0.rightbright.Brightness;
using unitrix0.rightbright.Brightness.Calculators;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Services.Brightness;
using unitrix0.rightbright.Services.CurveCalculation;
using unitrix0.rightbright.Services.MonitorAPI;
using unitrix0.rightbright.Settings;
using unitrix0.rightbright.TrayIcon;
using unitrix0.rightbright.Windows;
using unitrix0.rightbright.Windows.ViewModel;

namespace unitrix0.rightbright
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private TaskbarIcon _notifyIcon;
        private IBrightnessController _brightnessController;
        private TrayIconViewModel _notifyIconViewModel;

        protected override Window CreateShell()
        {
            InitTasbarIcon();

            _brightnessController = Container.Resolve<IBrightnessController>();
            _brightnessController.Run();
            return null;
        }

        private void InitTasbarIcon()
        {
            _notifyIcon = (TaskbarIcon) FindResource("NotifyIcon");
            if(_notifyIcon == null) return;
            
            _notifyIconViewModel = (TrayIconViewModel) _notifyIcon.DataContext;
            
            _notifyIcon.TrayContextMenuOpen += (sender, args) =>
                Debug.Print(((unitrix0.rightbright.TrayIcon.TrayIconViewModel)((System.Windows.FrameworkElement)sender).DataContext).PauseLabel);
            //_notifyIconViewModel.PauseAction = () =>
            //{
            //    _brightnessController.PauseSettingBrightness = !_brightnessController.PauseSettingBrightness;
            //    _notifyIconViewModel.PauseLabel = _brightnessController.PauseSettingBrightness ? "Fortsetzen" : "Pause";
            //};
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMonitorEnummerationService, MonitorEnummerationService>();
            containerRegistry.RegisterSingleton<IMonitorService, MonitorService>();
            containerRegistry.RegisterSingleton<IBrightnessCalculator, ProgressiveBrightnessCalculator>();
            containerRegistry.RegisterSingleton<IBrightnessController, BrightnessController>();
            containerRegistry.RegisterSingleton<ISensorService, SensorService>();
            containerRegistry.RegisterSingleton<ISensorRepo, SensorService>();
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
            _notifyIcon.Dispose();
        }
    }
}
