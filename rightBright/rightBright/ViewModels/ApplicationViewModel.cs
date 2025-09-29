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
    private readonly IBrightnessController _brightnessController = null!;

    public bool Suspend
    {
        get => _brightnessController.PauseSettingBrightness;
        set => _brightnessController.PauseSettingBrightness = value;
    }

    public Action? OnOpenMainWindow;
    [ObservableProperty] private WindowIcon? _appIcon;
    
    public ApplicationViewModel()
    {
    }
    
    public ApplicationViewModel(IBrightnessController brightnessController)
    {
        _brightnessController = brightnessController;
        SetAppIcon();
    }

    private void SetAppIcon()
    {
        var assemblyName= Assembly.GetExecutingAssembly().GetName().Name;
        var assetUri = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? $"avares://{assemblyName}/Assets/AppIcon 16 dark.ico"
            : $"avares://{assemblyName}/Assets/AppIcon 16 white.ico";
        
        AppIcon = new WindowIcon(AssetLoader.Open(new Uri(assetUri)));
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
        _brightnessController.PauseSettingBrightness = Suspend;
        base.OnPropertyChanged(e);
    }
}
