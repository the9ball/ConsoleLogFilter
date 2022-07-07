﻿using Microsoft.Extensions.Logging;

namespace ConsoleLogFilter;

/// <summary>
/// Provide ConsoleLogFilter
/// </summary>
internal class ConsoleLogFilterLoggerProvider : ILoggerProvider
{
    private readonly List<IDisposable> _disposables = new();

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
            using var writeStream = File.Open(logTemporaryFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);

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

    /// <inheritdoc/>
    ILogger ILoggerProvider.CreateLogger(string categoryName)
    {
        var innerLogger = _innerProvider.CreateLogger(categoryName);
        var logger = new ConsoleLogFilterLogger(categoryName, innerLogger, _onEntry, _logFilter);
        _disposables.Add(logger);
        return logger;
    }

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        _logFilter.Dispose();

        _innerProvider.Dispose();

        foreach (var d in _disposables) d.Dispose();
        _disposables.Clear();

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        _writingTask.GetAwaiter().GetResult();
    }
}

