using System.Threading.Tasks;

namespace rightBright.Services.Autostart;

public interface IAutostartService
{
    bool IsSupported { get; }
    Task<bool> SetAutostartAsync(bool enabled);
}
