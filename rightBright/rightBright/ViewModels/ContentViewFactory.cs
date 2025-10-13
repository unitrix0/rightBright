using System;

namespace rightBright.ViewModels;

public class ContentViewFactory(Func<Type, MainWindowContentViewModel> factory)
{
    private readonly Func<Type, MainWindowContentViewModel> _factory = factory;

    public MainWindowContentViewModel GetMainWindowContentViewModel<T>() where T: MainWindowContentViewModel
    {
        var viewModel = factory(typeof(T));
        return viewModel;
    }
}
