using System.ComponentModel;

namespace rightBright.Services.LoadingState;

public interface ILoadingMonitorStateService : INotifyPropertyChanged
{
    bool IsLoading { get; set; }
}

