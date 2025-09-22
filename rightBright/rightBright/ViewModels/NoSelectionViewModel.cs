using CommunityToolkit.Mvvm.ComponentModel;

namespace rightBright.ViewModels;

public partial class NoSelectionViewModel : MainViewContentViewModel
{
    [ObservableProperty] private string _message = "";
}
