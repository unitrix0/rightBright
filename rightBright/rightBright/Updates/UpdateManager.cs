using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using rightBright.Settings;

namespace rightBright.Updates;

public static class UpdateManager
{
    // Keep in sync with the WiX Burn bundle output name produced in pack-windows.ps1.
    private const string SetupAssetName = "rightBrightSetup.exe";

    // Keep in sync with your GitHub repository.
    private const string GitHubOwner = "unitrix0";
    private const string GitHubRepo = "rightBright";

    private static readonly Regex VersionRegex =
        new(@"\d+(\.\d+){1,3}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static async Task CheckAndApplyUpdatesAsync(CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsWindows())
            return;

        // Throttle network checks to once per day.
        var settings = AppSettings.Load();
        var nowUtc = DateTime.UtcNow;
        if (settings.LastUpdateCheckUtc is { } lastCheck &&
            nowUtc - lastCheck < TimeSpan.FromDays(1))
            return;

        settings.LastUpdateCheckUtc = nowUtc;
        try
        {
            settings.Save();
        }
        catch
        {
            // Don't block app start if settings can't be persisted.
        }

        var currentVersion = GetCurrentAppVersion();
        if (currentVersion is null)
            return;

        var latest = await GetLatestReleaseAsync(cancellationToken);
        if (latest is null)
            return;

        var latestVersion = ParseReleaseVersion(latest.tag_name);
        if (latestVersion is null || latest.assets is null)
            return;

        if (latestVersion <= currentVersion)
            return;

        var setupDownloadUrl = latest.assets
            .FirstOrDefault(a => string.Equals(a.name, SetupAssetName, StringComparison.OrdinalIgnoreCase))
            ?.browser_download_url;

        // Fallback if asset naming differs.
        setupDownloadUrl ??= latest.assets
            .FirstOrDefault(a => a.name?.EndsWith("Setup.exe", StringComparison.OrdinalIgnoreCase) == true)
            ?.browser_download_url;

        if (string.IsNullOrWhiteSpace(setupDownloadUrl))
            return;

        var setupPath = await DownloadFileAsync(setupDownloadUrl, SetupAssetName, cancellationToken);
        if (string.IsNullOrWhiteSpace(setupPath))
            return;

        // MSI/Burn will own creating the Run key + launching after finalize.
        // Exit current process so files aren't locked during upgrade.
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = setupPath,
                Arguments = "-quiet -norestart",
                UseShellExecute = true,
                CreateNoWindow = true,
                // If elevation is required, Windows will prompt.
                Verb = "runas"
            });

            // Don't block on installer; just let it run in the background and exit.
            Environment.Exit(0);
        }
        catch
        {
            // If we can't spawn the installer, don't prevent the app from running.
        }
    }

    private static Version? GetCurrentAppVersion()
    {
        try
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetName().Version;
        }
        catch
        {
            return null;
        }
    }

    private static Version? ParseReleaseVersion(string? tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            return null;

        var match = VersionRegex.Match(tagName);
        if (!match.Success)
            return null;

        var parsed = Version.TryParse(match.Value, out var v) ? v : null;
        if (parsed is null)
            return null;

        // Normalize to major/minor/build (Version.CompareTo works even with 2 parts, but be explicit).
        return new Version(parsed.Major, parsed.Minor, Math.Max(parsed.Build, 0));
    }

    private static async Task<GitHubRelease?> GetLatestReleaseAsync(CancellationToken cancellationToken)
    {
        using var http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        http.DefaultRequestHeaders.UserAgent.ParseAdd("rightBright-updater");
        http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

        var url = $"https://api.github.com/repos/{GitHubOwner}/{GitHubRepo}/releases/latest";

        try
        {
            using var response = await http.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<GitHubRelease>(stream, cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    private static async Task<string?> DownloadFileAsync(string url, string suggestedName, CancellationToken cancellationToken)
    {
        try
        {
            using var http = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            http.DefaultRequestHeaders.UserAgent.ParseAdd("rightBright-updater");

            using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var tempDir = Path.Combine(Path.GetTempPath(), "rightBright-updater");
            Directory.CreateDirectory(tempDir);

            var uniqueName = $"{Path.GetFileNameWithoutExtension(suggestedName)}_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)}_{Guid.NewGuid():N}.exe";
            var tempPath = Path.Combine(tempDir, uniqueName);

            await using var fs = File.Create(tempPath);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await stream.CopyToAsync(fs, cancellationToken);
            return tempPath;
        }
        catch
        {
            return null;
        }
    }

    private sealed class GitHubRelease
    {
        public string? tag_name { get; set; }
        public GitHubAsset[]? assets { get; set; }
    }

    private sealed class GitHubAsset
    {
        public string? name { get; set; }
        public string? browser_download_url { get; set; }
    }
}

