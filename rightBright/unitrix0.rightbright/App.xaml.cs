using Hardcodet.Wpf.TaskbarNotification;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Windows;
using unitrix0.rightbright.Brightness;
using unitrix0.rightbright.Brightness.Calculators;
using unitrix0.rightbright.Monitors;
using unitrix0.rightbright.Sensors;
using unitrix0.rightbright.Services.Brightness;
using unitrix0.rightbright.Services.CurveCalculation;
using unitrix0.rightbright.Services.MonitorAPI;
using unitrix0.rightbright.Services.TrayIcon;
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
        private TaskbarIcon? _notifyIcon;
        private IBrightnessController? _brightnessController;

        protected override Window CreateShell()
        {
            _notifyIcon = (TaskbarIcon?)FindResource("NotifyIcon");
            
            _brightnessController = Container.Resolve<IBrightnessController>();
            _brightnessController.Run();
            return null!;
        }


        protected override void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterSingleton<IMonitorEnummerationService, MonitorEnummerationService>();
            container.RegisterSingleton<IMonitorService, MonitorService>();
            container.RegisterSingleton<IBrightnessCalculator, ProgressiveBrightnessCalculator>();
            container.RegisterSingleton<IBrightnessController, BrightnessController>();
            container.RegisterSingleton<ISensorService, SensorService>();
            container.RegisterSingleton<ISensorRepo, SensorRepo>();
            container.RegisterSingleton<ISetBrightnessService, SetBrightnessService>();
            container.RegisterSingleton<ISettings>(Settings.Settings.Load);
            container.RegisterSingleton<ICurveCalculationService, CurveCalculationService>();
            container.RegisterSingleton<IDeviceChangedNotificationService, DeviceChangedNotificationService>();
            container.RegisterSingleton<IPowerNotificationService, PowerNotificationService>();
            container.Register<ITrayIcon>(() => new TrayIconService(_notifyIcon!));
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var settings = Container.Resolve<ISettings>();
            if (_brightnessController?.ConnectedSensor != null && settings != null)
            {
                settings.LastUsedSensor.MaxValue = _brightnessController.ConnectedSensor.MaxValue;
                settings.LastUsedSensor.MinValue = _brightnessController.ConnectedSensor.MinValue;
                TrySaveSettings(settings);
            }

            _notifyIcon?.Dispose();
        }

        private void TrySaveSettings(ISettings settings)
        {
            try
            {
                settings.Save();
                //throw new Exception("test", new Exception("Test sub"));
            }
            catch (Exception ex)
            {
                _notifyIcon?.ShowBalloonTip("Error", "Settings konnten nicht gespeicher werden. Prüfen Sie die Ereignisanzeige für Details", BalloonIcon.Error);
                //using (var eventlog = new EventLog("Application"))
                //{
                //    eventlog.Source = nameof(rightbright);
                //    eventlog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                //}
            }
        }
    }
}
