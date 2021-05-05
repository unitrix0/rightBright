using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System.Windows;
using unitrix0.rightbright.Brightness;
using unitrix0.rightbright.Windows;

namespace unitrix0.rightbright.TrayIcon
{
    public class TrayIconViewModel : BindableBase
    {
        private const string PauseLabelConst = "Pause";
        private const string ContinueLabelConst = "Fortsetzen";

        private string _pauseLabel;
        private readonly IBrightnessController _brigthnessController;

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
            var app = (PrismApplication)Application.Current;
            _brigthnessController = app.Container.Resolve<IBrightnessController>();
        }

        private void PauseBrightnessAdjustment()
        {
            _brigthnessController.PauseSettingBrightness = !_brigthnessController.PauseSettingBrightness;
            PauseLabel = PauseLabel == PauseLabelConst ? ContinueLabelConst : PauseLabelConst;
        }

        private void ExitApplicationCmd()
        {
            Application.Current.Shutdown();
        }

        private void ShowWindowCmd()
        {
            if (Application.Current.MainWindow == null ||
                Application.Current.MainWindow.GetType() != typeof(MainWindow))
                Application.Current.MainWindow = new MainWindow();

            Application.Current.MainWindow?.Show();
        }
    }
}