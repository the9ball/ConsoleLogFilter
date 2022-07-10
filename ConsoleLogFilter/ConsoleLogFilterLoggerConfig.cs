namespace the9ball.ConsoleLogFilter;

/// <summary>
/// <see cref="ConsoleLogFilterLoggerExtensions"/>用のConfiguration
/// </summary>
/// <param name="settingFilePath">Filter setting file path.</param>
/// <param name="logTemporaryFilePath">Temporary file path to reloading.</param>
public record struct ConsoleLogFilterLoggerConfig(
    string settingFilePath,
    string? logTemporaryFilePath = null
    );
