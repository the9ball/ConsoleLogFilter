using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Sample;
using the9ball.ConsoleLogFilter;

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        var innerProvider = new ConsoleLoggerProvider(new OptionsMonitor(new()));

        logging.ClearProviders();
        logging.AddConsoleLogFilterLogger(
            innerProvider,
            new ConsoleLogFilterLoggerConfig("../../../../setting.txt", characterColor: Color.Red, backgroundColor: Color.Cyan)
            );
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Sample.SampleApp>();
    })
    .Build()
    .RunAsync();

