using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System.Runtime.Loader;

namespace Markdig.Extensions.Xmd.CSCode;

public class CSCodeOptions
{
    internal bool DebugMode => (int)MinimumLogLevel <= (int)LogLevel.Debug;

    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;

    public string WorkingDirectory { get; set; }

    public string[] Arguments { get; internal set; }

    public OptimizationLevel? OptimizationLevel { get; set; }

    public bool NoCache { get; set; } = false;

    public string[] References { get; set; }

    public string[] Usings { get; set; }

    public AssemblyLoadContext AssemblyLoadContext { get; set; }

}
