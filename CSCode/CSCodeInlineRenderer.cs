using Dotnet.Script.Core;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdig.Extensions.Xmd.CSCode;

internal class CSCodeInlineRenderer : HtmlObjectRenderer<CSCodeInline>
{
    private readonly CSCodeRenderer _codeRenderer;
    private readonly CSCodeOptions _options;
    private readonly MarkdownPipeline _pipeline;

    internal CSCodeInlineRenderer(CSCodeRenderer codeRenderer, CSCodeOptions options, MarkdownPipeline pipeline)
    {
        _codeRenderer = codeRenderer;
        _options = options;
        _pipeline = pipeline;
    }

    protected override async void Write(HtmlRenderer renderer, CSCodeInline obj)
    {
        try
        {
            var (errors, content) = await _codeRenderer.WriteAsync(obj.SourceCode, true);
            renderer.Write($"{content}");

            if (errors?.Any() ?? false)
            {
                renderer.Write(string.Join("<br>\r\n", errors));
            }
        }
        catch (Exception ex)
        {
            renderer.Write(Markdown.ToHtml(_codeRenderer.BuildMarkdownExceptionMessage(ex, _options.DebugMode), _pipeline));
        }
    }
}