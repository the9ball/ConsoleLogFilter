using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sample;

/// <summary>
/// Sample service
/// </summary>
internal class SampleApp : IHostedService
{
    private static readonly LogLevel[] LogLevels = (LogLevel[])Enum.GetValues(typeof(LogLevel));

    private readonly ILogger _logger;

    public SampleApp(ILogger<SampleApp> logger) => _logger = logger;

    static string[] chars = new[]
    {
        "🤔",
        "あ",
        "r",
        "5",
    };

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var random = new Random();

        static string LongString(Random r)
        {
            var len = r.Next(64, 128);
            return string.Join("🍞", Enumerable.Repeat(0, len).Select(_ => chars[r.Next(chars.Length)]));
        }

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var logLevel = LogLevels[random.Next(LogLevels.Length)];
                //Console.WriteLine("LogLevel -> " + logLevel);
                _logger.Log(logLevel, "message {0} : {1}", logLevel, LongString(random));
                await Task.Delay(TimeSpan.FromSeconds(0.1), cancellationToken);

                try
                {
                    try
                    {
                        throw new Exception("test");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("hoge", ex);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "exception");
                }
            }
        }
        catch (OperationCanceledException) { }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(StopAsync));
        return Task.CompletedTask;
    }
}
