using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace rightBright.Theme;

/// <summary>
/// Reads the color tokens declared in Theme/Palette.axaml so that custom-drawn controls share
/// the palette with the XAML views. The fallback keeps those controls renderable in the XAML
/// previewer, where the application resources are not available.
/// </summary>
internal static class ThemePalette
{
    public static Color Resolve(string key, string fallback)
    {
        if (Application.Current is { } app &&
            app.TryGetResource(key, null, out var value) &&
            value is Color color)
        {
            return color;
        }

        return Color.Parse(fallback);
    }

    public static Color WithAlpha(Color color, byte alpha) =>
        Color.FromArgb(alpha, color.R, color.G, color.B);
}
