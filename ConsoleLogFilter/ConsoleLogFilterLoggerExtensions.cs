using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleLogFilter;

public static class ConsoleLogFilterLoggerExtensions
{
    /// <summary>
    /// Add <see cref="ConsoleLogFilterLoggerProvider"/>
    /// </summary>
    public static ILoggingBuilder AddConsoleLogFilterLogger(this ILoggingBuilder builder, ILoggerProvider innerProvider, string settingFilePath, string logTemporaryFilePath)
    {
        builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLogFilterLoggerProvider>(x => new ConsoleLogFilterLoggerProvider(innerProvider, logTemporaryFilePath, settingFilePath)));
        return builder;
    }

    /// <summary>
    /// Add <see cref="ConsoleLogFilterLoggerProvider"/>
    /// </summary>
    public static ILoggingBuilder AddConsoleLogFilterLogger(this ILoggingBuilder builder, ILoggerProvider innerProvider, string settingFilePath)
        => builder.AddConsoleLogFilterLogger(innerProvider, settingFilePath, Path.GetTempFileName());
}
