using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;
using Avalonia.Controls;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Brightness;
using rightBright.Services.Autostart;
using rightBright.Services.LoadingState;
using rightBright.Settings;
using Serilog;

namespace rightBright.ViewModels;

public partial class ApplicationViewModel : ViewModelBase
{
    private readonly IBrightnessController? _brightnessController;
    private readonly ILoadingMonitorStateService _loadingMonitorStateService;
    private readonly IAutostartService? _autostartService;
    private readonly ISettings? _settings;
    private WindowIcon? _normalIcon;
    private WindowIcon? _loadingIcon;
    [ObservableProperty] private WindowIcon? _appIcon;
    [ObservableProperty] private bool _isLoadingDisplays;
    [ObservableProperty] private bool _autostartEnabled;

    public Action? OnOpenMainWindow;
    public bool IsAutostartSupported => _autostartService?.IsSupported ?? false;


    public bool Suspend
    {
        get => _brightnessController?.PauseSettingBrightness ?? false;
        set
        {
            if (_brightnessController != null)
            {
                _brightnessController.PauseSettingBrightness = value;
            }
        }
    }


    public ApplicationViewModel()
    {
        SetAppIcon();
        _loadingMonitorStateService = new LoadingMonitorStateService();
    }

    public ApplicationViewModel(IBrightnessController brightnessController,
        ILoadingMonitorStateService loadingMonitorStateService,
        IAutostartService autostartService,
        ISettings settings)
    {
        _brightnessController = brightnessController;
        _loadingMonitorStateService = loadingMonitorStateService;
        _autostartService = autostartService;
        _settings = settings;

        _autostartEnabled = settings.AutostartEnabled;

        // Preserve installer / registry autostart on Windows:
        // - First run: no settings.json yet — mirror Run key into settings.
        // - Reinstall: Inno recreates the Run key while AppData may still have
        //   AutostartEnabled=false; without this, startup sync would delete that key.
        if (OperatingSystem.IsWindows() &&
            _autostartService is WindowsAutostartService windowsAutostartService &&
            settings is AppSettings appSettings)
        {
            var regEnabled = windowsAutostartService.GetAutostartEnabled();
            if (!appSettings.SettingsFileExisted)
            {
                _autostartEnabled = regEnabled;
                _settings.AutostartEnabled = _autostartEnabled;
                _settings.Save();
            }
            else if (regEnabled && !appSettings.AutostartEnabled)
            {
                Log.Information(
                    "[Autostart][Windows] Registry Run key present while settings had autostart off; aligning settings with registry (e.g. after reinstall)");
                _autostartEnabled = true;
                _settings.AutostartEnabled = true;
                _settings.Save();
            }
        }
        SetAppIcon();
        SubscribeToLoadingState();
    }

    private void SubscribeToLoadingState()
    {
        _loadingMonitorStateService.PropertyChanged += OnLoadingMonitorStateChanged;
        IsLoadingDisplays = _loadingMonitorStateService.IsLoading;
    }

    private void OnLoadingMonitorStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ILoadingMonitorStateService.IsLoading))
        {
            IsLoadingDisplays = _loadingMonitorStateService.IsLoading;
        }
    }

    private static bool IsWindows11()
    {
        // Use registry-based detection so we aren't affected by OS version "lying"
        // when an application manifest only declares Windows 10 compatibility.
        if (!OperatingSystem.IsWindows()) return false;

        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var currentBuildString = key?.GetValue("CurrentBuild")?.ToString()
                                     ?? key?.GetValue("CurrentBuildNumber")?.ToString();

            if (int.TryParse(currentBuildString, out var currentBuild))
            {
                // Windows 11 starts at build 22000.
                return currentBuild >= 22000;
            }

            // Fallback: OSDescription is typically "Microsoft Windows 11 ..."
            var osDescription = RuntimeInformation.OSDescription;
            return osDescription.Contains("Windows 11", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private void SetAppIcon()
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var useDarkIcon = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || IsWindows11();

        var normalIconUri = useDarkIcon
            ? $"avares://{assemblyName}/Assets/app_icon_new.ico"
            : $"avares://{assemblyName}/Assets/AppIcon 16 white.ico";

        var loadingIconUri = useDarkIcon
            ? $"avares://{assemblyName}/Assets/app_icon_detecting.ico"
            : $"avares://{assemblyName}/Assets/AppIcon 16 dark.ico";

        _normalIcon = new WindowIcon(AssetLoader.Open(new Uri(normalIconUri)));
        _loadingIcon = new WindowIcon(AssetLoader.Open(new Uri(loadingIconUri)));

        AppIcon = _normalIcon;
    }

    private void SetLoadingIcon()
    {
        if (_loadingIcon != null)
        {
            AppIcon = _loadingIcon;
        }
    }

    private void SetNormalIcon()
    {
        if (_normalIcon != null)
        {
            AppIcon = _normalIcon;
        }
    }

    [RelayCommand]
    private void OpenMainWindow()
    {
        OnOpenMainWindow?.Invoke();
    }

    [RelayCommand]
    private async Task ToggleAutostartAsync()
    {
        if (_autostartService is null || _settings is null) return;

        var desired = !AutostartEnabled;
        var granted = await _autostartService.SetAutostartAsync(desired);
        AutostartEnabled = granted && desired;
        _settings.AutostartEnabled = AutostartEnabled;
        _settings.Save();
    }

    public async Task SyncAutostartWithPortalAsync()
    {
        if (_autostartService is null || _settings is null) return;

        // Windows: best-effort reconciliation on every startup, including removal
        // when the user disables the toggle.
        if (_autostartService is WindowsAutostartService windowsAutostartService)
        {
            var desired = _settings.AutostartEnabled;
            var granted = await windowsAutostartService.SetAutostartAsync(desired);

            if (!desired) return;

            if (!granted)
            {
                Log.Warning("[Autostart][Windows] Failed to re-register on startup; disabling setting");
                AutostartEnabled = false;
                _settings.AutostartEnabled = false;
                _settings.Save();
            }

            return;
        }

        // Flatpak: keep portal behavior (only re-register when enabled) to avoid
        // unnecessary portal calls.
        if (_autostartService is not { IsSupported: true }) return;
        if (!_settings.AutostartEnabled) return;

        var portalGranted = await _autostartService.SetAutostartAsync(true);
        if (!portalGranted)
        {
            Log.Warning("[Autostart] Portal denied re-registration on startup; disabling setting");
            AutostartEnabled = false;
            _settings.AutostartEnabled = false;
            _settings.Save();
        }
    }

    [RelayCommand]
    private void EndProgram()
    {
        Environment.Exit(0);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IsLoadingDisplays) when IsLoadingDisplays:
                SetLoadingIcon();
                break;
            case nameof(IsLoadingDisplays):
                SetNormalIcon();
                break;
            case nameof(Suspend):
            {
                if (_brightnessController != null)
                {
                    _brightnessController.PauseSettingBrightness = Suspend;
                }

                break;
            }
        }

        base.OnPropertyChanged(e);
    }
}
