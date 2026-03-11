using System;

namespace rightBright.ViewModels;

public class ContentViewFactory
{
    private readonly Func<Type, MainWindowContentViewModel> _factory;

    public ContentViewFactory(Func<Type, MainWindowContentViewModel> factory)
    {
        _factory = factory;
    }

    public MainWindowContentViewModel GetMainWindowContentViewModel<T>() where T: MainWindowContentViewModel
    {
        var viewModel = _factory(typeof(T));
        return viewModel;
    }
}
