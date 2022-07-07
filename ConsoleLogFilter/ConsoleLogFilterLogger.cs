using Microsoft.Extensions.Logging;

namespace ConsoleLogFilter;

/// <summary>
/// Logger to filter console logs
/// </summary>
internal class ConsoleLogFilterLogger : ILogger, IDisposable
{
    private readonly string _categoryName;
    private readonly ILogger _innerLogger;
    private readonly AwaitableQueue<Entry> _outputQueue;
    private readonly ILogFilter _logFilter;

    private readonly AwaitableQueue<Entry> _readingQueue = new();

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _readingTask;

    /// <summary></summary>
    public ConsoleLogFilterLogger(string categoryName, ILogger innerLogger, AwaitableQueue<Entry> outputQueue, ILogFilter logFilter)
    {
        _categoryName = categoryName;
        _innerLogger = innerLogger;
        _outputQueue = outputQueue;
        _logFilter = logFilter;

        _cancellationTokenSource = new();
        _readingTask = ReadLoop(_cancellationTokenSource.Token);
    }

    /// <summary>
    /// ReadingLoop
    /// </summary>
    private async Task ReadLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var entry = await _readingQueue.DequeueAsync(cancellationToken);

                // Filtering message
                if (_logFilter.Filter(entry.Message) is not { } message) continue;

                _innerLogger.Log(entry.LogLevel, entry.EventId, message);
            }
        }
        catch (OperationCanceledException) { }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        _readingTask.GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    IDisposable ILogger.BeginScope<TState>(TState state) => _innerLogger.BeginScope(state);

    /// <inheritdoc/>
    bool ILogger.IsEnabled(LogLevel logLevel) => _innerLogger.IsEnabled(logLevel);

    /// <inheritdoc/>
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        var entry = new Entry(_categoryName, logLevel, eventId, message);

        _readingQueue.Enqueue(entry);
        _outputQueue.Enqueue(entry);
    }
}
