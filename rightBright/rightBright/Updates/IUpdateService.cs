using System.Threading;
using System.Threading.Tasks;

namespace rightBright.Updates;

public interface IUpdateService
{
    void StartPeriodicChecks();
    void StopPeriodicChecks();
    Task CheckForUpdatesAsync(bool force = false, CancellationToken ct = default);
}
