using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Brightness;

namespace rightBright.ViewModels;

public partial class ApplicationViewModel : ViewModelBase
{
    private IBrightnessController? _brightnessController;
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
    
    public void SetBrightnessController(IBrightnessController brightnessController)
    {
        _brightnessController = brightnessController;
    }

    public Action? OnOpenMainWindow;
    [ObservableProperty] private WindowIcon? _appIcon;
    
    [ObservableProperty]
    private bool _isLoadingDisplays;
    
    public ApplicationViewModel()
    {
        SetAppIcon();
    }
    
    public ApplicationViewModel(IBrightnessController brightnessController)
    {
        _brightnessController = brightnessController;
        SetAppIcon();
    }

    private void SetAppIcon()
    {
        var assemblyName= Assembly.GetExecutingAssembly().GetName().Name;
        var normalIconUri = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? $"avares://{assemblyName}/Assets/app_icon_new.ico"
            : $"avares://{assemblyName}/Assets/AppIcon 16 white.ico";
        
        var loadingIconUri = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
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
        if (e.PropertyName == nameof(IsLoadingDisplays))
        {
            if (IsLoadingDisplays)
            {
                SetLoadingIcon();
            }
            else
            {
                SetNormalIcon();
            }
        }
        
        if (e.PropertyName == nameof(Suspend))
        {
            if (_brightnessController != null)
            {
                _brightnessController.PauseSettingBrightness = Suspend;
            }
        }
        
        base.OnPropertyChanged(e);
    }
}
