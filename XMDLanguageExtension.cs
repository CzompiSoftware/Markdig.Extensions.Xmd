using Markdig.Renderers;
using Markdig.Extensions.Xmd.CSCode;
using Markdig.Extensions.Xmd.Alert;
using Dotnet.Script.Core;
using Dotnet.Script.DependencyModel.Logging;
using System;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Markdig.Extensions.Xmd.Logging;

namespace Markdig.Extensions.Xmd;

public class XmdLanguageExtension : IMarkdownExtension
{
    private readonly CSCodeOptions _options;
    private readonly ScriptConsole _scriptConsole;
    private readonly LogFactory _logFactory;
    private readonly ScriptCompiler _scriptCompiler;

    public XmdLanguageExtension(CSCodeOptions options)
    {
        _options = options;
        _logFactory = CreateLogFactory(options.MinimumLogLevel, options.DebugMode);
        _scriptCompiler = new(_logFactory, !_options.NoCache);
        _scriptConsole = ScriptConsole.Default;
        
        Globals.AddToGlobalScript(_options.References, _options.Usings);
    }

    internal static Func<LogLevel, bool, LogFactory> CreateLogFactory = (verbosity, debugMode) =>
    {
        return LogHelper.CreateLogFactory(debugMode ? LogLevel.Trace : verbosity);
    };
    public XmdLanguageExtension()
    {
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.InlineParsers.ReplaceOrAdd<CSCodeInlineParser>(new CSCodeInlineParser());
        pipeline.InlineParsers.ReplaceOrAdd<CSCodeBlockParser>(new CSCodeBlockParser());
        pipeline.InlineParsers.ReplaceOrAdd<AlertBlockParser>(new AlertBlockParser(pipeline));
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        var codeRenderer = new CSCodeRenderer(_scriptCompiler, _scriptConsole, _options, _logFactory, pipeline);
        renderer.ObjectRenderers.ReplaceOrAdd<CSCodeInlineRenderer>(new CSCodeInlineRenderer(codeRenderer, _options, pipeline));
        renderer.ObjectRenderers.ReplaceOrAdd<CSCodeBlockRenderer>(new CSCodeBlockRenderer(codeRenderer, _options, pipeline));
        renderer.ObjectRenderers.ReplaceOrAdd<AlertBlockRenderer>(new AlertBlockRenderer(pipeline));
    }

}
