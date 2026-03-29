using System.Reflection;
using Avalonia.Controls;

namespace rightBright.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        if (version is not null)
            Title = $"rightBright - {version.Major}.{version.Minor}.{version.Build}";
    }
}