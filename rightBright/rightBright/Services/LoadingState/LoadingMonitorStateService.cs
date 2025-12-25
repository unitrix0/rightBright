using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace rightBright.Services.LoadingState;

public class LoadingMonitorStateService : ObservableObject, ILoadingMonitorStateService
{
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
}

