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
using Serilog;

namespace rightBright.Updates;

public class UpdateService : IUpdateService, IDisposable
{
    private const string SetupAssetName = "rightBrightSetup.exe";
    private const string GitHubOwner = "unitrix0";
    private const string GitHubRepo = "rightBright";

    private static readonly Regex VersionRegex =
        new(@"\d+(\.\d+){1,3}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly ILogger _logger;
    private readonly ISettings _settings;
    private Timer? _periodicTimer;
    private int _checking; // 0 = idle, 1 = running (for reentrancy guard)
    private int _installing; // 0 = idle, 1 = running
    private readonly Lock _updateStateLock = new();
    private string? _availableVersion;
    private string? _availableSetupDownloadUrl;

    public bool IsUpdateAvailable { get; private set; }
    public string? AvailableVersion
    {
        get
        {
            lock (_updateStateLock)
            {
                return _availableVersion;
            }
        }
    }

    public bool IsInstallingUpdate => Interlocked.CompareExchange(ref _installing, 0, 0) == 1;
    public event EventHandler? UpdateAvailabilityChanged;

    public UpdateService(ISettings settings, ILogger logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public void StartPeriodicChecks()
    {
        if (!OperatingSystem.IsWindows())
        {
            _logger.Information("[UpdateService] Periodic update checks disabled (non-Windows OS)");
            return;
        }

        var intervalHours = _settings.UpdateCheckIntervalHours;
        if (intervalHours <= 0) intervalHours = 6;

        var interval = TimeSpan.FromHours(intervalHours);
        _logger.Information("[UpdateService] Starting periodic update checks every {IntervalHours}h", intervalHours);

        _periodicTimer = new Timer(_ => _ = CheckForUpdatesAsync(force: false), null, TimeSpan.Zero, interval);
    }

    public void StopPeriodicChecks()
    {
        _logger.Information("[UpdateService] Stopping periodic update checks");
        _periodicTimer?.Change(Timeout.Infinite, Timeout.Infinite);
    }

    public async Task CheckForUpdatesAsync(bool force = false, CancellationToken ct = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            _logger.Debug("[UpdateService] Skipping update check (non-Windows OS)");
            return;
        }

        // Reentrancy guard -- only one check at a time.
        if (Interlocked.CompareExchange(ref _checking, 1, 0) != 0)
        {
            _logger.Debug("[UpdateService] Check already in progress, skipping");
            return;
        }

        try
        {
            await CheckAndApplyUpdatesAsync(force, ct);
        }
        finally
        {
            Interlocked.Exchange(ref _checking, 0);
        }
    }

    private async Task CheckAndApplyUpdatesAsync(bool force, CancellationToken ct)
    {
        var currentVersion = GetCurrentAppVersion();
        if (currentVersion is null)
        {
            _logger.Warning("[UpdateService] Could not determine current app version; aborting check");
            return;
        }

        _logger.Information("[UpdateService] Update check started (current: {CurrentVersion}, force: {Force})",
            currentVersion, force);

        if (!force)
        {
            var settings = AppSettings.Load();
            var nowUtc = DateTime.UtcNow;
            if (settings.LastUpdateCheckUtc is { } lastCheck &&
                nowUtc - lastCheck < TimeSpan.FromDays(1))
            {
                _logger.Information(
                    "[UpdateService] Skipping check -- last check was {LastCheck:u}, less than 24h ago",
                    lastCheck);
                return;
            }

            settings.LastUpdateCheckUtc = nowUtc;
            try
            {
                settings.Save();
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "[UpdateService] Failed to persist LastUpdateCheckUtc");
            }
        }

        var latest = await GetLatestReleaseAsync(ct);
        if (latest is null)
        {
            _logger.Warning("[UpdateService] Could not fetch latest release from GitHub");
            return;
        }

        var latestVersion = ParseReleaseVersion(latest.tag_name);
        if (latestVersion is null || latest.assets is null)
        {
            _logger.Warning("[UpdateService] Could not parse version from tag '{Tag}'", latest.tag_name);
            return;
        }

        _logger.Information("[UpdateService] Latest release: {LatestVersion} (tag: {Tag})",
            latestVersion, latest.tag_name);

        if (latestVersion <= currentVersion)
        {
            _logger.Information("[UpdateService] Already up-to-date ({Current} >= {Latest})",
                currentVersion, latestVersion);
            SetAvailableUpdateState(isAvailable: false, version: null, setupDownloadUrl: null);
            return;
        }

        _logger.Information("[UpdateService] Newer version available: {Latest} (current: {Current})",
            latestVersion, currentVersion);

        var setupDownloadUrl = latest.assets
            .FirstOrDefault(a => string.Equals(a.name, SetupAssetName, StringComparison.OrdinalIgnoreCase))
            ?.browser_download_url;

        setupDownloadUrl ??= latest.assets
            .FirstOrDefault(a => a.name?.EndsWith("Setup.exe", StringComparison.OrdinalIgnoreCase) == true)
            ?.browser_download_url;

        if (string.IsNullOrWhiteSpace(setupDownloadUrl))
        {
            _logger.Warning("[UpdateService] No suitable installer asset found in release");
            return;
        }

        SetAvailableUpdateState(isAvailable: true, version: latestVersion.ToString(), setupDownloadUrl: setupDownloadUrl);
        _logger.Information("[UpdateService] Update available and waiting for manual install action");
    }

    public async Task InstallAvailableUpdateAsync(CancellationToken ct = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            _logger.Debug("[UpdateService] Install skipped (non-Windows OS)");
            return;
        }

        if (Interlocked.CompareExchange(ref _installing, 1, 0) != 0)
        {
            _logger.Debug("[UpdateService] Install already in progress, skipping");
            return;
        }

        try
        {
            string? setupDownloadUrl;
            lock (_updateStateLock)
            {
                setupDownloadUrl = _availableSetupDownloadUrl;
            }

            if (string.IsNullOrWhiteSpace(setupDownloadUrl))
            {
                _logger.Warning("[UpdateService] No pending installer URL available for manual install");
                return;
            }

            _logger.Information("[UpdateService] Downloading installer from {Url}", setupDownloadUrl);
            var setupPath = await DownloadFileAsync(setupDownloadUrl, SetupAssetName, ct);
            if (string.IsNullOrWhiteSpace(setupPath))
            {
                _logger.Error("[UpdateService] Installer download failed");
                return;
            }

            _logger.Information("[UpdateService] Installer downloaded to {Path}", setupPath);
            _logger.Information("[UpdateService] Launching installer and exiting application");
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = setupPath,
                Arguments = "-quiet -norestart",
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = "runas"
            });

            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[UpdateService] Failed to install available update");
        }
        finally
        {
            Interlocked.Exchange(ref _installing, 0);
            NotifyUpdateAvailabilityChanged();
        }
    }

    private void SetAvailableUpdateState(bool isAvailable, string? version, string? setupDownloadUrl)
    {
        var changed = false;
        lock (_updateStateLock)
        {
            if (IsUpdateAvailable != isAvailable ||
                !string.Equals(_availableVersion, version, StringComparison.Ordinal) ||
                !string.Equals(_availableSetupDownloadUrl, setupDownloadUrl, StringComparison.Ordinal))
            {
                IsUpdateAvailable = isAvailable;
                _availableVersion = version;
                _availableSetupDownloadUrl = setupDownloadUrl;
                changed = true;
            }
        }

        if (changed)
        {
            NotifyUpdateAvailabilityChanged();
        }
    }

    private void NotifyUpdateAvailabilityChanged()
    {
        try
        {
            UpdateAvailabilityChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "[UpdateService] Failed to notify update state change");
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

        return new Version(parsed.Major, parsed.Minor, Math.Max(parsed.Build, 0));
    }

    private async Task<GitHubRelease?> GetLatestReleaseAsync(CancellationToken ct)
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("rightBright-updater");
        http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

        // /releases/latest excludes prereleases; list all and pick the first non-draft.
        var url = $"https://api.github.com/repos/{GitHubOwner}/{GitHubRepo}/releases?per_page=15";

        try
        {
            using var response = await http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.Warning("[UpdateService] GitHub API returned {StatusCode}", response.StatusCode);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            var releases = await JsonSerializer.DeserializeAsync<GitHubRelease[]>(stream, cancellationToken: ct);

            var latest = releases?.FirstOrDefault(r => r.draft != true);
            if (latest is null)
                _logger.Warning("[UpdateService] No non-draft releases found on GitHub");

            return latest;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "[UpdateService] GitHub API request failed");
            return null;
        }
    }

    private async Task<string?> DownloadFileAsync(string url, string suggestedName, CancellationToken ct)
    {
        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
            http.DefaultRequestHeaders.UserAgent.ParseAdd("rightBright-updater");

            using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.Warning("[UpdateService] Download returned {StatusCode} for {Url}",
                    response.StatusCode, url);
                return null;
            }

            var tempDir = Path.Combine(Path.GetTempPath(), "rightBright-updater");
            Directory.CreateDirectory(tempDir);

            var uniqueName =
                $"{Path.GetFileNameWithoutExtension(suggestedName)}_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)}_{Guid.NewGuid():N}.exe";
            var tempPath = Path.Combine(tempDir, uniqueName);

            await using var fs = File.Create(tempPath);
            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            await stream.CopyToAsync(fs, ct);
            return tempPath;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[UpdateService] File download failed for {Url}", url);
            return null;
        }
    }

    public void Dispose()
    {
        _periodicTimer?.Dispose();
    }

    private sealed class GitHubRelease
    {
        public string? tag_name { get; set; }
        public bool? draft { get; set; }
        public bool? prerelease { get; set; }
        public GitHubAsset[]? assets { get; set; }
    }

    private sealed class GitHubAsset
    {
        public string? name { get; set; }
        public string? browser_download_url { get; set; }
    }
}
