using Microsoft.Extensions.Logging;

namespace ConsoleLogFilter;

/// <summary>
/// Logger to filter console logs
/// </summary>
internal class ConsoleLogFilterLogger : ILogger
{
    private readonly ILogger _innerLogger;

    /// <summary></summary>
    public ConsoleLogFilterLogger(ILogger innerLogger) => _innerLogger = innerLogger;

    /// <inheritdoc/>
    IDisposable ILogger.BeginScope<TState>(TState state) => _innerLogger.BeginScope(state);

    /// <inheritdoc/>
    bool ILogger.IsEnabled(LogLevel logLevel) => _innerLogger.IsEnabled(logLevel);

    /// <inheritdoc/>
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _innerLogger.Log(logLevel, eventId, state, exception, formatter);
    }
}
