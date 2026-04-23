using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Win32;
using Serilog;

namespace rightBright.Services.Autostart;

/// <summary>
/// Manages per-user Windows login autostart via
/// `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`.
/// </summary>
[SupportedOSPlatform("windows")]
public class WindowsAutostartService : IAutostartService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string RunValueName = "rightBright";

    private readonly ILogger _logger;

    public bool IsSupported => true;

    public WindowsAutostartService(ILogger logger)
    {
        _logger = logger;
    }

    public Task<bool> SetAutostartAsync(bool enabled)
    {
        try
        {
            if (enabled)
            {
                using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath);

                var exePath = GetExecutablePath();
                if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
                {
                    _logger.Warning("[Autostart][Windows] Invalid exe path: {ExePath}", exePath);
                    return Task.FromResult(false);
                }

                // `Run` expects the value to be a command line.
                var commandLine = $"\"{exePath}\"";
                key!.SetValue(RunValueName, commandLine, RegistryValueKind.String);
                return Task.FromResult(true);
            }
            else
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
                if (key is null) return Task.FromResult(true); // Already "disabled".

                var existing = key.GetValue(RunValueName) as string;
                if (existing is null)
                {
                    return Task.FromResult(true); // Already "disabled".
                }

                key.DeleteValue(RunValueName, throwOnMissingValue: false);
                return Task.FromResult(true);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[Autostart][Windows] Failed to set autostart={Enabled}", enabled);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Checks whether a non-empty Run entry exists for the app.
    /// </summary>
    public bool GetAutostartEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            if (key is null) return false;

            var existing = key.GetValue(RunValueName) as string;
            return !string.IsNullOrWhiteSpace(existing);
        }
        catch
        {
            return false;
        }
    }

    private static string? GetExecutablePath()
    {
        // Environment.ProcessPath is the most reliable in modern .NET.
        var processPath = Environment.ProcessPath;
        if (!string.IsNullOrWhiteSpace(processPath))
        {
            return processPath;
        }

        try
        {
            return Process.GetCurrentProcess().MainModule?.FileName;
        }
        catch
        {
            return null;
        }
    }
}

