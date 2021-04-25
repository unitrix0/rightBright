using Prism.Commands;
using System.Windows;
using unitrix0.rightbright.Windows;

namespace unitrix0.rightbright.TrayIcon
{
    public class TrayIconViewModel
    {
        public DelegateCommand ShowWindowCommand => new DelegateCommand(ShowWindowCmd, () => Application.Current != null);

        public DelegateCommand ExitApplicationCommand => new DelegateCommand(ExitApplicationCmd);
        

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