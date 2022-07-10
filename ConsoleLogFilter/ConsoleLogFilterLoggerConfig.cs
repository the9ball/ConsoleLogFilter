namespace the9ball.ConsoleLogFilter;

/// <summary>
/// Coloring
/// </summary>
public enum Color
{
    Black = 0,
    Red = 1,
    Green = 2,
    Yellow = 3,
    Blue = 4,
    Purple = 5,
    Cyan = 6,
    White = 7,
}

/// <summary>
/// <see cref="ConsoleLogFilterLoggerExtensions"/>用のConfiguration
/// </summary>
/// <param name="settingFilePath">Filter setting file path.</param>
/// <param name="logTemporaryFilePath">Temporary file path to reloading.</param>
/// <param name="characterColor">Character hilight color.</param>
/// <param name="backgroundColor">Background hilight color.</param>
public record struct ConsoleLogFilterLoggerConfig(
    string settingFilePath,
    string? logTemporaryFilePath = null,
    Color characterColor = Color.Black,
    Color backgroundColor = Color.White
    );
