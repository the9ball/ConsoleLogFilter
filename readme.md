ConsoleLogFilter
===

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

- [Getting Started](#getting-started)
- [License](#license)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

Getting Started
---
For Console application [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host).

1. Add package reference for YourApplication.
  ```
  <PackageReference Include="the9ball.ConsoleLogFilter" Version="{input newer}" />
  ```
2. Call `AddConsoleLogFilterLogger` in `ConfigureLogging`.
    ```
    .ConfigureLogging(logging =>
    {
        var innerProvider = new ConsoleLoggerProvider(new OptionsMonitor(new()));

        logging.ClearProviders();
        logging.AddConsoleLogFilterLogger(innerProvider, new ConsoleLogFilterLoggerConfig("../../../../setting.txt"));
    })
    ```
3. Create setting file.(e.g. `setting.txt`)
    ```
    First-line is filter. Excluding logs if not match. Regular expression.
    Second-line is hilight. Regular expression.
    ```
    - More information : [.NET regular expressions](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions)
4. Run application.
5. Edit setting file.(optional)

Nuget page
---
the9ball.ConsoleLogFilter : https://www.nuget.org/packages/the9ball.ConsoleLogFilter/

License
---
This library is licensed under the MIT License.
