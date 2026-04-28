using System;
using System.Threading;
using System.Threading.Tasks;

namespace rightBright.Updates;

public interface IUpdateService
{
    bool IsUpdateAvailable { get; }
    string? AvailableVersion { get; }
    bool IsInstallingUpdate { get; }
    event EventHandler? UpdateAvailabilityChanged;

    void StartPeriodicChecks();
    void StopPeriodicChecks();
    void RestartPeriodicChecks();
    Task CheckForUpdatesAsync(bool force = false, CancellationToken ct = default);
    Task InstallAvailableUpdateAsync(CancellationToken ct = default);
}
