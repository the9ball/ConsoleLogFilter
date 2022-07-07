using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sample;

/// <summary>
/// Sample service
/// </summary>
internal class SampleApp : IHostedService
{
    private readonly ILogger _logger;

    public SampleApp(ILogger<SampleApp> logger) => _logger = logger;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(StartAsync));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(StopAsync));
        return Task.CompletedTask;
    }
}
