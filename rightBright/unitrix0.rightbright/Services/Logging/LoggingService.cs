using System;
using System.IO;

namespace unitrix0.rightbright.Services.Logging;

public class LoggingService : ILoggingService
{
    private const string LogfileName = "rightbright.log";

    public void WriteInformation(string msg)
    {
        File.AppendAllText(LogfileName, $"{DateTime.Now:g} INF\t{msg}\n");
    }

    public void WriteWarning(string msg)
    {
        File.AppendAllText(LogfileName, $"{DateTime.Now:g} WRN\t{msg}\n");
    }

    public void WriteError(string msg)
    {
        File.AppendAllText(LogfileName, $"{DateTime.Now:g} ERR\t{msg}\n");
    }
}