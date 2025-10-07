using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace unitrix0.rightbright.Services.Logging;

public class LoggingService : ILoggingService
{
    private const string LogfileName = "rightbright.log";
    private readonly string _logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),Assembly.GetEntryAssembly()!.GetName().Name!, LogfileName);

    public void WriteInformation(string msg)
    {
        var line = $"{DateTime.Now:g} INF\t{msg}\n";
        if (Debugger.IsAttached) Debug.Print(line);
        File.AppendAllText(_logFilePath, line);
    }

    public void WriteWarning(string msg)
    {
        var line = $"{DateTime.Now:g} WRN\t{msg}\n";
        if (Debugger.IsAttached) Debug.Print(line);
        File.AppendAllText(_logFilePath, line);
    }

    public void WriteError(string msg)
    {
        var line = $"{DateTime.Now:g} ERR\t{msg}\n";
        if (Debugger.IsAttached) Debug.Print(line);
        File.AppendAllText(_logFilePath, line);
    }
}
