﻿using System.Text.RegularExpressions;

namespace ConsoleLogFilter;

/// <summary>
/// Filter
/// </summary>
internal interface ILogFilter
{
    /// <summary>
    /// Filtering message
    /// </summary>
    /// <returns>
    /// If filtered null
    /// If hilight hilghted string
    /// </returns>
    string? Filter(string message);
}

/// <summary>
/// Filter
/// </summary>
internal class LogFilter : ILogFilter, IDisposable
{
    /// <summary>
    /// Create <see cref="LogFilter"/> instance.
    /// </summary>
    public static LogFilter Create(string settingFilePath)
    {
        var logFilter = new LogFilter(settingFilePath);
        logFilter.Read();
        return logFilter;
    }

    private readonly string _settingFilePath;
    private readonly FileSystemWatcher _watcher;

    private string? _filterString;
    private Regex? _filterRegex;

    private string? _hilightString;
    private Regex? _hilightRegex;

    /// <summary>
    /// Replacement pattern to hilighting
    /// </summary>
    /// <remarks>
    /// TODO: color setting
    /// </remarks>
    const string _hilightPattern = "\u001b[47m\u001b[30m$&\u001b[0m";

    /// <summary>
    /// Setting file reloaded.
    /// </summary>
    public event Action? OnReload;

    /// <summary></summary>
    private LogFilter(string settingFilePath)
    {
        if (!Path.IsPathRooted(settingFilePath))
        {
            // To full path
            settingFilePath = Path.Combine(Directory.GetCurrentDirectory(), settingFilePath);
        }

        // Remove parent(..) paths
        _settingFilePath = Path.GetFullPath(settingFilePath);

        var d = Path.GetDirectoryName(settingFilePath) is { } x && !string.IsNullOrEmpty(x) ? x : Directory.GetCurrentDirectory();
        var f = Path.GetFileName(settingFilePath);
        _watcher = new(d, f)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true,
        };
        _watcher.Changed += (_, __) => Read();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        _watcher.Dispose();
    }

    /// <summary>
    /// Read setting file.
    /// </summary>
    private void Read()
    {
        string?[] lines = new string?[2];
        using (var s = File.Open(_settingFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var r = new StreamReader(s))
        {
            lines[0] = r.ReadLine();
            lines[1] = r.ReadLine();
        }

        if (Read(lines)) OnReload?.Invoke();
    }

    /// <summary>
    /// Read settings
    /// </summary>
    /// <returns>changed</returns>
    private bool Read(string?[] lines)
    {
        bool changed = false;

        {
            var f = lines.Length >= 1 ? lines[0] : null;
            if (_filterString != f)
            {
                _filterString = f;
                _filterRegex = string.IsNullOrEmpty(f) ? null : new(f, RegexOptions.IgnoreCase);
                changed = true;
            }
        }

        {
            var h = lines.Length >= 2 ? lines[1] : null;
            if (_hilightString != h)
            {
                _hilightString = h;
                _hilightRegex = string.IsNullOrEmpty(h) ? null : new(h, RegexOptions.IgnoreCase);
                changed = true;
            }
        }

        return changed;
    }

    /// <inheritdoc cref="ILogFilter.Filter"/>
    public string? Filter(string message)
    {
        // Return null if not matched.
        if (_filterRegex is { } f && !f.IsMatch(message)) return null;

        // hilighting
        if (_hilightRegex is { } h) message = h.Replace(message, _hilightPattern);

        return message;
    }
}
