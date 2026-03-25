using System;
using System.Diagnostics;
using System.IO;

namespace rightBright.Services.Logging;

public class LoggingService : ILoggingService
{
    private const string LogfileName = "rightbright.log";

    private readonly string _logfilePath = Path
        .Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LogfileName);

    public void WriteInformation(string msg)
    {
        var line = $"{DateTime.Now:g} INF\t{msg}\n";
        if (Debugger.IsAttached) Debug.Print(line.TrimEnd('\n'));
        File.AppendAllText(_logfilePath, line);
    }

    public void WriteWarning(string msg)
    {
        var line = $"{DateTime.Now:g} WRN\t{msg}\n";
        if (Debugger.IsAttached) Debug.Print(line.TrimEnd('\n'));
        File.AppendAllText(_logfilePath, line);
    }

    public void WriteError(string msg)
    {
        var line = $"{DateTime.Now:g} ERR\t{msg}\n";
        if (Debugger.IsAttached) Debug.Print(line.TrimEnd('\n'));
        File.AppendAllText(_logfilePath, line);
    }
}
