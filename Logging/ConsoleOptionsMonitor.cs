using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Markdig.Extensions.Xmd.Logging;

internal class ConsoleOptionsMonitor : IOptionsMonitor<ConsoleLoggerOptions>
{
    private readonly ConsoleLoggerOptions _consoleLoggerOptions;

    public ConsoleOptionsMonitor()
    {
        _consoleLoggerOptions = new ConsoleLoggerOptions()
        {
            LogToStandardErrorThreshold = LogLevel.Trace
        };
    }

    public ConsoleLoggerOptions CurrentValue => _consoleLoggerOptions;

    public ConsoleLoggerOptions Get(string name) => _consoleLoggerOptions;

    public IDisposable OnChange(Action<ConsoleLoggerOptions, string> listener)
    {
        return null;
    }
}