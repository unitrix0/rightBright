namespace rightBright.Services.DBus;

record QGuiApplicationProperties
{
    public string ApplicationDisplayName { get; set; } = "";
    public string DesktopFileName { get; set; } = "";
    public string PlatformName { get; set; } = "";
    public bool QuitOnLastWindowClosed { get; set; }
}
