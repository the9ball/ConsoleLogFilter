using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Sample;

/// <inheritdoc cref="IOptionsMonitor{TOptions}"/>
internal class OptionsMonitor : IOptionsMonitor<ConsoleLoggerOptions>
{
    public OptionsMonitor(ConsoleLoggerOptions currentValue) => CurrentValue = currentValue;

    /// <inheritdoc/>
    public ConsoleLoggerOptions CurrentValue { get; }

    /// <inheritdoc/>
    public ConsoleLoggerOptions Get(string name) => CurrentValue;

    /// <inheritdoc/>
    public IDisposable? OnChange(Action<ConsoleLoggerOptions, string> listener) => null;
}

