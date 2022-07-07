using Microsoft.Extensions.Logging;

namespace ConsoleLogFilter;

internal class ConsoleLogFilterLoggerProvider : ILoggerProvider
{
    private readonly ILoggerProvider _innerProvider;

    public ConsoleLogFilterLoggerProvider(ILoggerProvider innerProvider) => _innerProvider = innerProvider;

    ILogger ILoggerProvider.CreateLogger(string categoryName)
    {
        var innerLogger = _innerProvider.CreateLogger(categoryName);
        return new ConsoleLogFilterLogger(innerLogger);
    }

    void IDisposable.Dispose() => _innerProvider.Dispose();
}

