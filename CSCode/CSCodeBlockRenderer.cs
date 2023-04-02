using Markdig.Renderers;
using Markdig.Renderers.Html;
using System;
using System.Linq;

namespace Markdig.Extensions.Xmd.CSCode;

public class CSCodeBlockRenderer : HtmlObjectRenderer<CSCodeBlock>
{
    private readonly CSCodeRenderer _codeRenderer;
    private readonly CSCodeOptions _options;
    private readonly MarkdownPipeline _pipeline;

    internal CSCodeBlockRenderer(CSCodeRenderer codeRenderer, CSCodeOptions options, MarkdownPipeline pipeline)
    {
        _codeRenderer = codeRenderer;
        _options = options;
        _pipeline = pipeline;
    }

    protected override async void Write(HtmlRenderer renderer, CSCodeBlock obj)
    {
        try
        {
            var (errors, content) = await _codeRenderer.WriteAsync(obj.SourceCode, false);
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