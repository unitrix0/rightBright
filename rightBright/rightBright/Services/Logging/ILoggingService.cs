namespace rightBright.Services.Logging;

public interface ILoggingService
{
    void WriteInformation(string msg);
    void WriteWarning(string msg);
    void WriteError(string msg);
}