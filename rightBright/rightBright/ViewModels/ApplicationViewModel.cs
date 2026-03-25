using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Avalonia.Controls;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Brightness;
using rightBright.Services.LoadingState;

namespace rightBright.ViewModels;

public partial class ApplicationViewModel : ViewModelBase
{
    private readonly IBrightnessController? _brightnessController;
    private readonly ILoadingMonitorStateService _loadingMonitorStateService;
    private WindowIcon? _normalIcon;
    private WindowIcon? _loadingIcon;

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

    public Action? OnOpenMainWindow;
    [ObservableProperty] private WindowIcon? _appIcon;

    [ObservableProperty] private bool _isLoadingDisplays;

    public ApplicationViewModel()
    {
        SetAppIcon();
        _loadingMonitorStateService = new LoadingMonitorStateService();
    }

    public ApplicationViewModel(IBrightnessController brightnessController,
        ILoadingMonitorStateService loadingMonitorStateService)
    {
        _brightnessController = brightnessController;
        _loadingMonitorStateService = loadingMonitorStateService;
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
    private void ToggleSuspendUpdating()
    {
        Suspend = !Suspend;
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
