using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample;
using the9ball.ConsoleLogFilter;

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, logging) =>
    {
        var innerProvider = new OriginalLoggerProvider();

        logging.ClearProviders();
#if true
        logging.AddConsoleLogFilterLogger(
            innerProvider,
            new ConsoleLogFilterLoggerConfig("../../../../setting.txt", characterColor: Color.Red, backgroundColor: Color.Cyan),
            context.Configuration
            );
#else
        logging.Services.AddSingleton<ILoggerProvider>(innerProvider);
#endif
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Sample.SampleApp>();
    })
    .Build()
    .RunAsync();

