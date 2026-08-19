using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace rightBright.Views.Controls;

public partial class ViewHeader : UserControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ViewHeader, string?>(nameof(Title));

    public static readonly StyledProperty<ICommand?> CloseCommandProperty =
        AvaloniaProperty.Register<ViewHeader, ICommand?>(nameof(CloseCommand));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public ICommand? CloseCommand
    {
        get => GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public ViewHeader()
    {
        InitializeComponent();
    }
}
