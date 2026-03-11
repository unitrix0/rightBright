using CommunityToolkit.Mvvm.ComponentModel;

namespace rightBright.ViewModels;

public partial class NoSelectionContentViewModel : MainWindowContentViewModel
{
    [ObservableProperty] private string _message = "";
}
