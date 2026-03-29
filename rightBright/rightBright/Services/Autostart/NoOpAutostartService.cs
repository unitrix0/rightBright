using System.Threading.Tasks;

namespace rightBright.Services.Autostart;

/// <summary>
/// Fallback for environments where portal-based autostart is not available
/// (e.g. running outside a Flatpak, or on Windows).
/// </summary>
public class NoOpAutostartService : IAutostartService
{
    public bool IsSupported => false;

    public Task<bool> SetAutostartAsync(bool enabled) => Task.FromResult(false);
}
