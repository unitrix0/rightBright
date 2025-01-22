using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Windows;
using unitrix0.rightbright.Brightness;
using unitrix0.rightbright.Settings;
using unitrix0.rightbright.Windows;

namespace unitrix0.rightbright.TrayIcon
{
    public class TrayIconViewModel : BindableBase
    {
        private const string PauseLabelConst = "Pause";
        private const string ContinueLabelConst = "Fortsetzen";
        private string _pauseLabel = PauseLabelConst;

        public string PauseLabel
        {
            get => _pauseLabel;
            set => SetProperty(ref _pauseLabel, value);
        }

        public DelegateCommand ShowWindowCommand => new(ShowWindowCmd, () => Application.Current != null);

        public DelegateCommand ExitApplicationCommand => new(ExitApplicationCmd);

        public DelegateCommand PauseCommand => new(PauseBrightnessAdjustment, () => true);


        public TrayIconViewModel()
        {
            PauseLabel = PauseLabelConst;
        }

        private void PauseBrightnessAdjustment()
        {
            //_brigthnessController.PauseSettingBrightness = !_brigthnessController.PauseSettingBrightness;
            PauseLabel = PauseLabel == PauseLabelConst ? ContinueLabelConst : PauseLabelConst;
        }

        private void ExitApplicationCmd()
        {
            TrySaveSensorData();
            Application.Current.Shutdown();
        }

        private void TrySaveSensorData()
        {
            var application = Application.Current as PrismApplication;
            var brightnessController = application?.Container.Resolve<IBrightnessController>();
            var settings = application?.Container.Resolve<ISettings>();

            if (brightnessController?.ConnectedSensor == null || settings == null) return;

            settings.LastUsedSensor.MaxValue = brightnessController.ConnectedSensor.MaxValue;
            settings.LastUsedSensor.MinValue = brightnessController.ConnectedSensor.MinValue;
            try
            {
                settings.Save();
            }
            catch (Exception ex)
            {
                ShowWindowCmd();
                MessageBox.Show(Application.Current.MainWindow!,
                    $"Settings konnten nicht gespeicher werden: {ex.Message}",
                    "rightBright", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ShowWindowCmd()
        {
            if (Application.Current.MainWindow == null ||
                Application.Current.MainWindow.GetType() != typeof(MainWindow))
                Application.Current.MainWindow = new MainWindow();

            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
        }
    }
}
