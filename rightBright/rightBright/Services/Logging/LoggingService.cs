using System;
using System.Diagnostics;
using System.IO;

namespace unitrix0.rightbright.Services.Logging;

public class LoggingService : ILoggingService
{
    private const string LogfileName = "rightbright.log";

    public void WriteInformation(string msg)
    {
        var line = $"{DateTime.Now:g} INF\t{msg}\n";
        if (Debugger.IsAttached) Debug.Print(line);
        File.AppendAllText(LogfileName, line);
    }

    public void WriteWarning(string msg)
    {
        var line = $"{DateTime.Now:g} WRN\t{msg}\n";
        if (Debugger.IsAttached) Debug.Print(line);
        File.AppendAllText(LogfileName, line);
    }

    public void WriteError(string msg)
    {
        var line = $"{DateTime.Now:g} ERR\t{msg}\n";
        if (Debugger.IsAttached) Debug.Print(line);
        File.AppendAllText(LogfileName, line);
    }
}
