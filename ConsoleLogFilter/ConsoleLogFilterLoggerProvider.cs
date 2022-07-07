using Microsoft.Extensions.Logging;

namespace ConsoleLogFilter;

internal class ConsoleLogFilterLoggerProvider : ILoggerProvider
{
    private readonly List<IDisposable> _disposables = new();

    private readonly ILoggerProvider _innerProvider;

    private readonly AwaitableQueue<Entry> _onEntry = new();

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _writingTask;

    public ConsoleLogFilterLoggerProvider(ILoggerProvider innerProvider, string logTemporaryFilePath)
    {
        _innerProvider = innerProvider;

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

    ILogger ILoggerProvider.CreateLogger(string categoryName)
    {
        var innerLogger = _innerProvider.CreateLogger(categoryName);
        var logger = new ConsoleLogFilterLogger(categoryName, innerLogger, _onEntry);
        _disposables.Add(logger);
        return logger;
    }

    void IDisposable.Dispose()
    {
        _innerProvider.Dispose();

        foreach (var d in _disposables) d.Dispose();
        _disposables.Clear();

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        _writingTask.GetAwaiter().GetResult();
    }
}

