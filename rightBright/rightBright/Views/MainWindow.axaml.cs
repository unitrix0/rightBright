using System.Reflection;
using Avalonia.Controls;
using rightBright.Localization;

namespace rightBright.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var versionString = Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        if (versionString is not null)
        {
            // Strip build metadata ("+commitHash") but keep prerelease suffix ("-rc.1")
            var metaIndex = versionString.IndexOf('+');
            if (metaIndex >= 0)
                versionString = versionString[..metaIndex];

            Title = Texts.Format(nameof(Texts.MainWindowTitleFormat), versionString);
        }
        else
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            if (version is not null)
                Title = Texts.Format(nameof(Texts.MainWindowTitleFormat),
                    $"{version.Major}.{version.Minor}.{version.Build}");
        }
    }
}
