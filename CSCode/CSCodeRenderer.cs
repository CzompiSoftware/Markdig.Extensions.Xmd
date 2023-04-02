using System;
using Microsoft.CodeAnalysis;
using System.IO;
using Dotnet.Script.DependencyModel.Logging;
using Dotnet.Script.Core;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Dotnet.Script.DependencyModel.Context;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Markdig.Extensions.Xmd.CSCode;

internal class CSCodeRenderer
{
    private readonly LogFactory _logFactory;
    private readonly ScriptCompiler _scriptCompiler;
    private readonly ScriptConsole _scriptConsole;
    private readonly CSCodeOptions _options;
    private readonly MarkdownPipeline _pipeline;

    public CSCodeRenderer(ScriptCompiler scriptCompiler, ScriptConsole scriptConsole, CSCodeOptions options, LogFactory logFactory, MarkdownPipeline pipeline)
    {
        _logFactory = logFactory;
        _scriptCompiler = scriptCompiler;
        _scriptConsole = scriptConsole;
        _options = options;
        _pipeline = pipeline;
    }

    internal async Task<(ScriptState, ScriptCompilationContext<object>, List<string>)> ExecuteAsync(string script, string previousScript = null)
    {
        List<string> errors = new();
        ScriptContext context = new(SourceText.From((previousScript ?? Globals.GlobalScript) ?? script), _options.WorkingDirectory ?? Directory.GetCurrentDirectory(), _options.Arguments, null, _options.OptimizationLevel ?? OptimizationLevel.Debug, ScriptMode.Eval);
        ScriptCompilationContext<object> compilationContext = _scriptCompiler.CreateCompilationContext<object, CommandLineScriptGlobals>(context);
        var globals = new CommandLineScriptGlobals(_scriptConsole.Out, CSharpObjectFormatter.Instance);
        ScriptState result=null;
        try
        {
            result = await compilationContext.Script.ContinueWith(script).RunAsync(globals);
        }
        catch (Exception ex)
        {
            errors.Add(Markdown.ToHtml(BuildMarkdownExceptionMessage(ex, _options.DebugMode), _pipeline));
        }
        return (result, compilationContext, errors);
    }

    internal async Task<(List<string>, string)> WriteAsync(string script, bool inline, string previousScript = null)
    {
        List<string> errors = new();
        string content = null;
        var (state, compilationContext, executeErrors) = await ExecuteAsync(script, previousScript);

        // Display errors
        if (compilationContext.Errors.Any())
        {
            foreach (var error in compilationContext.Errors)
            {
                errors.Add(Markdown.ToHtml(BuildMarkdownExceptionMessage(error), _pipeline));
            }
        }

        if (executeErrors.Any())
        {
            errors.AddRange(executeErrors);
        }

        // Render code result
        if (XmdDocument.Instance.ReportObject != null)
        {
            if (XmdDocument.Instance.ReportObject is HtmlReportObject htmlReportObject)
            {
                content = htmlReportObject.Html;
            }
            else if (XmdDocument.Instance.ReportObject is MarkdownReportObject markdownReportObject)
            {
                var markdown = Markdown.ToHtml(markdownReportObject.Markdown, _pipeline);
                if (inline)
                {
                    if (markdown.StartsWith("<p>"))
                        markdown = markdown[3..];
                    if (markdown.EndsWith("</p>\n"))
                        markdown = markdown[..^5];
                }

                content = markdown;
            }
        }
        XmdDocument.Instance.Reset();
        return (errors, content);
    }


    internal string BuildMarkdownExceptionMessage(Diagnostic diagnostic)
    {
        var minLevel = (int)_options.MinimumLogLevel - 1;
        if (minLevel < (int)diagnostic.Severity) return "";
        var level = diagnostic.Severity.ToString().ToLowerInvariant();
        string message = $"]>xmdcscmp-{level}<\r\n";
        message += $"#### XmdCSCompiler: *{level}* severity error occurred\r\n```\r\n";
        message += diagnostic.GetMessage();

        return $"{message.Replace(Environment.NewLine, $"\r\n] ")}\r\n```\r\n";
    }

    internal string BuildMarkdownExceptionMessage(Exception exception, bool stackTrace)
    {
        var minLevel = (int)_options.MinimumLogLevel;
        if (minLevel > (int)LogLevel.Error) return "";

        string message = $"]>xmdcscmp-fatal<\r\n";
        message += "#### XmdCSCompiler: *fatal* severity error occurred\r\n```\r\n";
        message += exception.Message;

        if (exception is FileNotFoundException fileNotFoundException)
        {
            message += " (" + fileNotFoundException.FileName + ")";
        }
        if (stackTrace)
        {
            message += Environment.NewLine;
            message += exception.StackTrace;
        }

        return $"{message.Replace(Environment.NewLine, $"\r\n] ")}\r\n```\r\n";
    }
}