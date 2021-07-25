namespace unitrix0.rightbright.Services.TrayIcon
{
    public interface ITrayIcon
    {
        void ShowInformationBalloon(string titel, string msg);
        void ShowWarningBallon(string title, string msg);
    }
}