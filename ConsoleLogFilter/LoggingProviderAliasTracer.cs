using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace the9ball.ConsoleLogFilter;

/// <summary>
/// Tracing <see cref="ProviderAliasAttribute"/>
/// </summary>
internal class LoggingProviderAliasTracer : IConfigureOptions<LoggerFilterOptions>
{
    private readonly string _providerName;
    private readonly string _targetTypeName;
    private readonly IConfiguration _configuration;

    public LoggingProviderAliasTracer(string providerName, string targetTypeName, IConfiguration configuration)
    {
        _providerName = providerName;
        _targetTypeName = targetTypeName;
        _configuration = configuration;
    }

    void IConfigureOptions<LoggerFilterOptions>.Configure(LoggerFilterOptions options)
    {
        var logLevels = _configuration.GetSection("Logging").GetSection(_providerName).GetSection("LogLevel");
        foreach (var l in logLevels.GetChildren())
        {
            var key = l.Key != "Default" ? l.Key : null;
            options.Rules.Add(new(_targetTypeName, key, GetLogLevel(l.Value), null));
        }
    }

    private static LogLevel GetLogLevel(string value)
        => Enum.TryParse<LogLevel>(value, ignoreCase: true, out var logLevel) ? logLevel : throw new ArgumentException(value, nameof(value));
}

