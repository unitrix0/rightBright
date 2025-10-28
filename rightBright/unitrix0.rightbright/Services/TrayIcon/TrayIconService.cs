using H.NotifyIcon;
using H.NotifyIcon.Core;

namespace unitrix0.rightbright.Services.TrayIcon
{
    public class TrayIconService : ITrayIcon
    {
        private readonly TaskbarIcon _taskbarIcon;

        public TrayIconService(TaskbarIcon taskbarIcon)
        {
            _taskbarIcon = taskbarIcon;
        }

        public void ShowInformationBalloon(string titel, string msg)
        {
            _taskbarIcon.ShowNotification(titel, msg, NotificationIcon.Info);
        }

        public void ShowWarningBallon(string title, string msg)
        {
            _taskbarIcon.ShowNotification(title, msg, NotificationIcon.Error);
        }
    }
}
