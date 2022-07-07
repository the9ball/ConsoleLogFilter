﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleLogFilter;

public static class ConsoleLogFilterLoggerExtensions
{
    /// <summary>
    /// Add <see cref="ConsoleLogFilterLoggerProvider"/>
    /// </summary>
    public static ILoggingBuilder AddConsoleLogFilterLogger(this ILoggingBuilder builder, ILoggerProvider innerProvider)
    {
        builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLogFilterLoggerProvider>(x => new ConsoleLogFilterLoggerProvider(innerProvider)));
        return builder;
    }
}
