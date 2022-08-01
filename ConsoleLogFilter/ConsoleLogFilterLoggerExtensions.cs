using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace the9ball.ConsoleLogFilter;

public static class ConsoleLogFilterLoggerExtensions
{
    /// <summary>
    /// Add <see cref="ConsoleLogFilterLoggerProvider"/>
    /// </summary>
    public static ILoggingBuilder AddConsoleLogFilterLogger(this ILoggingBuilder builder, ILoggerProvider innerProvider, ConsoleLogFilterLoggerConfig loggerConfiguration, IConfiguration configuration)
        => _AddConsoleLogFilterLogger(builder, innerProvider, loggerConfiguration, configuration);

    /// <inheritdoc cref="AddConsoleLogFilterLogger(ILoggingBuilder, ILoggerProvider, ConsoleLogFilterLoggerConfig, IConfiguration)"/>
    /// <remarks>support old version</remarks>
    private static ILoggingBuilder _AddConsoleLogFilterLogger(ILoggingBuilder builder, ILoggerProvider innerProvider, ConsoleLogFilterLoggerConfig loggerConfiguration, IConfiguration? configuration)
    {
        if (configuration is not null) // for old version
        {
            if (innerProvider.GetType().GetCustomAttributes(typeof(ProviderAliasAttribute), true).FirstOrDefault() is ProviderAliasAttribute pa && pa is not null)
            {
                builder.Services.AddSingleton<IConfigureOptions<LoggerFilterOptions>, LoggingProviderAliasTracer>(
                    serviceProvier => new LoggingProviderAliasTracer(pa.Alias, typeof(ConsoleLogFilterLoggerProvider).FullName!, configuration));
            }

            builder.Services.AddSingleton<IConfigureOptions<LoggerFilterOptions>, LoggingProviderAliasTracer>(
                serviceProvier => new LoggingProviderAliasTracer(innerProvider.GetType().FullName!, typeof(ConsoleLogFilterLoggerProvider).FullName!, configuration));
        }

        builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLogFilterLoggerProvider>(x => new ConsoleLogFilterLoggerProvider(innerProvider, loggerConfiguration)));
        return builder;
    }

    /// <summary>
    /// Add <see cref="ConsoleLogFilterLoggerProvider"/>
    /// </summary>
    [Obsolete("Use ConsoleLogFilterLoggerConfig")]
    public static ILoggingBuilder AddConsoleLogFilterLogger(this ILoggingBuilder builder, ILoggerProvider innerProvider, ConsoleLogFilterLoggerConfig configuration)
        => _AddConsoleLogFilterLogger(builder, innerProvider, configuration, null);

    /// <summary>
    /// Add <see cref="ConsoleLogFilterLoggerProvider"/>
    /// </summary>
    [Obsolete("Use ConsoleLogFilterLoggerConfig")]
    public static ILoggingBuilder AddConsoleLogFilterLogger(this ILoggingBuilder builder, ILoggerProvider innerProvider, string settingFilePath, string logTemporaryFilePath)
        => builder.AddConsoleLogFilterLogger(innerProvider, new ConsoleLogFilterLoggerConfig(settingFilePath, logTemporaryFilePath: logTemporaryFilePath));

    /// <summary>
    /// Add <see cref="ConsoleLogFilterLoggerProvider"/>
    /// </summary>
    [Obsolete("Use ConsoleLogFilterLoggerConfig")]
    public static ILoggingBuilder AddConsoleLogFilterLogger(this ILoggingBuilder builder, ILoggerProvider innerProvider, string settingFilePath)
        => builder.AddConsoleLogFilterLogger(innerProvider, new ConsoleLogFilterLoggerConfig(settingFilePath, Path.GetTempFileName()));
}
