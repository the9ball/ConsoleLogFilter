using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace the9ball.ConsoleLogFilter;

public static class ConsoleLogFilterLoggerExtensions
{
    /// <summary>
    /// Add <see cref="ConsoleLogFilterLoggerProvider"/>
    /// </summary>
    public static ILoggingBuilder AddConsoleLogFilterLogger(this ILoggingBuilder builder, ILoggerProvider innerProvider, ConsoleLogFilterLoggerConfig configuration)
    {
        builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLogFilterLoggerProvider>(x => new ConsoleLogFilterLoggerProvider(innerProvider, configuration)));
        return builder;
    }

    /// <summary>
    /// Add <see cref="ConsoleLogFilterLoggerProvider"/>
    /// </summary>
    [Obsolete("Use ConsoleLogFilterLoggerConfig")]
    public static ILoggingBuilder AddConsoleLogFilterLogger(this ILoggingBuilder builder, ILoggerProvider innerProvider, string settingFilePath)
        => builder.AddConsoleLogFilterLogger(innerProvider, new ConsoleLogFilterLoggerConfig(settingFilePath, Path.GetTempFileName()));
}
