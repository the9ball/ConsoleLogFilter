using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace the9ball.ConsoleLogFilter;

/// <summary>
/// Provide ConsoleLogFilter
/// </summary>
internal class ConsoleLogFilterLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ConsoleLogFilterLogger> _loggerCache = new();

    private readonly ILoggerProvider _innerProvider;

    private readonly AwaitableQueue<Entry> _onEntry = new();

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _writingTask;

    private readonly LogFilter _logFilter;

    /// <summary></summary>
    public ConsoleLogFilterLoggerProvider(ILoggerProvider innerProvider, string logTemporaryFilePath, string settingFilePath)
    {
        _innerProvider = innerProvider;
        _logFilter = LogFilter.Create(settingFilePath);
        _logFilter.OnReload += () => Task.Run(() => ReloadLog(logTemporaryFilePath));

        _cancellationTokenSource = new();
        _writingTask = WriteLoop(logTemporaryFilePath, _onEntry, _cancellationTokenSource.Token);
    }

    /// <summary>
    /// ReadingLoop
    /// </summary>
    private async Task WriteLoop(string logTemporaryFilePath, AwaitableQueue<Entry> queue, CancellationToken cancellationToken)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine(logTemporaryFilePath);
            using var writeStream = File.Open(logTemporaryFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

            while (!cancellationToken.IsCancellationRequested)
            {
                var entry = await queue.DequeueAsync(cancellationToken);
                entry.Write(writeStream);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            File.Delete(logTemporaryFilePath);
        }
    }

    /// <summary>
    /// Reload log
    /// </summary>
    private void ReloadLog(string logTemporaryFilePath)
    {
        using var s = File.Open(logTemporaryFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        Console.Clear();

        while (s.Position != s.Length)
        {
            var entry = Entry.Read(s);
            if (!_loggerCache.TryGetValue(entry.CategoryName, out var logger)) continue;
            logger.Log(entry);
        }
    }

    /// <inheritdoc/>
    ILogger ILoggerProvider.CreateLogger(string categoryName)
    {
        var innerLogger = _innerProvider.CreateLogger(categoryName);
        var logger = new ConsoleLogFilterLogger(categoryName, innerLogger, _onEntry, _logFilter);
        _loggerCache.TryAdd(categoryName, logger);
        return logger;
    }

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        _logFilter.Dispose();

        _innerProvider.Dispose();

        foreach (var d in _loggerCache.Values) d.Dispose();
        _loggerCache.Clear();

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        _writingTask.GetAwaiter().GetResult();
    }
}

