using System.Globalization;
using System.Resources;

namespace rightBright.Localization;

public static class Texts
{
    private static readonly ResourceManager ResourceManager = new("rightBright.Resources.Strings", typeof(Texts).Assembly);

    public static string TrayOpen => Get(nameof(TrayOpen));
    public static string TrayPause => Get(nameof(TrayPause));
    public static string TrayAutostart => Get(nameof(TrayAutostart));
    public static string TrayCheckForUpdates => Get(nameof(TrayCheckForUpdates));
    public static string TrayExit => Get(nameof(TrayExit));
    public static string MainSensorLabel => Get(nameof(MainSensorLabel));
    public static string MainConnectButton => Get(nameof(MainConnectButton));
    public static string SidebarScreens => Get(nameof(SidebarScreens));
    public static string SidebarDetectMonitorsTooltip => Get(nameof(SidebarDetectMonitorsTooltip));
    public static string SidebarSearchingMonitors => Get(nameof(SidebarSearchingMonitors));
    public static string SidebarSensorLabel => Get(nameof(SidebarSensorLabel));
    public static string SidebarMinLabel => Get(nameof(SidebarMinLabel));
    public static string SidebarCurrentLabel => Get(nameof(SidebarCurrentLabel));
    public static string SidebarMaxLabel => Get(nameof(SidebarMaxLabel));
    public static string SidebarInstallUpdateButton => Get(nameof(SidebarInstallUpdateButton));
    public static string CurveActive => Get(nameof(CurveActive));
    public static string CurveMinBrightnessP0 => Get(nameof(CurveMinBrightnessP0));
    public static string CurveHundredPercentAtLuxP2 => Get(nameof(CurveHundredPercentAtLuxP2));
    public static string CurveApply => Get(nameof(CurveApply));
    public static string CurveApplyToActive => Get(nameof(CurveApplyToActive));
    public static string ChartBrightnessAxis => Get(nameof(ChartBrightnessAxis));
    public static string ChartLuxAxis => Get(nameof(ChartLuxAxis));
    public static string NoSelectionNoSensorsFound => Get(nameof(NoSelectionNoSensorsFound));
    public static string NoSelectionNoSensorConnected => Get(nameof(NoSelectionNoSensorConnected));
    public static string UpdateAvailableGeneric => Get(nameof(UpdateAvailableGeneric));
    public static string MainWindowTitleFormat => Get(nameof(MainWindowTitleFormat));
    public static string SettingsTitle => Get(nameof(SettingsTitle));
    public static string SettingsLanguage => Get(nameof(SettingsLanguage));
    public static string SettingsSensorPolling => Get(nameof(SettingsSensorPolling));
    public static string SettingsUpdateInterval => Get(nameof(SettingsUpdateInterval));
    public static string SettingsAutostart => Get(nameof(SettingsAutostart));

    public static string Format(string key, params object[] args)
    {
        var format = Get(key);
        return string.Format(CultureInfo.CurrentUICulture, format, args);
    }

    public static string Get(string key)
    {
        return ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
