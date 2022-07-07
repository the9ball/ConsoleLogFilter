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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var random = new Random();

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var logLevel = LogLevels[random.Next(LogLevels.Length)];
                Console.WriteLine("LogLevel -> " + logLevel);
                _logger.Log(logLevel, "message {0}", logLevel);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
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
