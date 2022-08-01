using Microsoft.Extensions.Logging;

namespace Sample;

/// <summary>
/// Dummy IDisposable
/// </summary>
internal class EmptyDisposable : IDisposable
{
    void IDisposable.Dispose() { }
}

internal class OriginalLogger : ILogger
{
    private readonly string _categoryName;

    public OriginalLogger(string categoryName) => _categoryName = categoryName;

    IDisposable ILogger.BeginScope<TState>(TState state) => new EmptyDisposable();

    bool ILogger.IsEnabled(LogLevel logLevel) => true;

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var text = DateTime.Now + "|" + formatter(state, exception);
        if (exception is not null) text += Environment.NewLine + exception.ToString();
        Console.WriteLine(text);
    }
}

[ProviderAlias("OriginalLogger")]
internal class OriginalLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new OriginalLogger(categoryName);

    public void Dispose() { }
}

