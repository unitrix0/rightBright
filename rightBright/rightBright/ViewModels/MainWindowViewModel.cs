using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace rightBright.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    [ObservableProperty] private string name = "value";
}
