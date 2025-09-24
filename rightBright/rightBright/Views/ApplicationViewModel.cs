using System;
using System.ComponentModel;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rightBright.Brightness;
using rightBright.ViewModels;

namespace rightBright.Views;

public partial class ApplicationViewModel : ViewModelBase
{
    private readonly IBrightnessController _brightnessController = null!;
    private readonly MainWindowViewModel _mainWindowViewModel = null!;

    public bool Suspend
    {
        get => _brightnessController.PauseSettingBrightness;
        set => _brightnessController.PauseSettingBrightness = value;
    }

    public Action? OnOpenMainWindow;
    
    public ApplicationViewModel()
    {
    }
    
    public ApplicationViewModel(IBrightnessController brightnessController, MainWindowViewModel mainWindowViewModel)
    {
        _brightnessController = brightnessController;
        _mainWindowViewModel = mainWindowViewModel;
    }
    
    [RelayCommand]
    private void OpenMainWindow()
    {
        OnOpenMainWindow?.Invoke();
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
