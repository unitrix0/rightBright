using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace rightBright.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<DisplayInfo> displays = [];

    public MainWindowViewModel()
    {
        if (Avalonia.Controls.Design.IsDesignMode) SeedDesignTimeData();
    }

    private void SeedDesignTimeData()
    {
        displays =
        [
            new DisplayInfo() { DeviceName = "Screen 1", ModelName = "Model" },
            new DisplayInfo() { DeviceName = "Screen 2", ModelName = "Model" }
        ];
    }
}
