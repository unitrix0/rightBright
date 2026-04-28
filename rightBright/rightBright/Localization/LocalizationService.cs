using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace rightBright.Localization;

public static class LocalizationService
{
    private static readonly HashSet<string> SupportedLanguages =
    [
        "en",
        "de"
    ];

    public static string ResolveLanguageCode(string? preferredLanguageCode)
    {
        if (!string.IsNullOrWhiteSpace(preferredLanguageCode))
        {
            var normalized = NormalizeLanguageCode(preferredLanguageCode);
            if (SupportedLanguages.Contains(normalized))
            {
                return normalized;
            }
        }

        var currentUiLanguage = NormalizeLanguageCode(CultureInfo.CurrentUICulture.Name);
        return SupportedLanguages.Contains(currentUiLanguage) ? currentUiLanguage : "en";
    }

    public static string ApplyLanguage(string? preferredLanguageCode)
    {
        var languageCode = ResolveLanguageCode(preferredLanguageCode);
        var culture = CultureInfo.GetCultureInfo(languageCode);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        return languageCode;
    }

    private static string NormalizeLanguageCode(string value)
    {
        return value.Split('-', '_', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault()?
            .ToLowerInvariant() ?? "en";
    }
}
