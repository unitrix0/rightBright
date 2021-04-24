using Prism.Commands;
using Prism.Mvvm;
using System.Windows;

namespace unitrix0.rightbright.TrayIcon
{
    public class TrayIconViewModel : BindableBase
    {
        public DelegateCommand ShowWindowCommand => new DelegateCommand(ShowWindowCmd, () => Application.Current != null);

        public DelegateCommand ExitApplicationCommand => new DelegateCommand(ExitApplicationCmd);

        private void ExitApplicationCmd()
        {
            Application.Current.Shutdown();
        }

        private void ShowWindowCmd()
        {
            Application.Current.MainWindow?.Show();
        }
    }
}