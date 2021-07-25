using Hardcodet.Wpf.TaskbarNotification;

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
            _taskbarIcon.ShowBalloonTip(titel, msg, BalloonIcon.Info);
        }

        public void ShowWarningBallon(string title, string msg)
        {
            _taskbarIcon.ShowBalloonTip(title, msg, BalloonIcon.Error);
        }
    }
}