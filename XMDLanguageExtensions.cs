using Markdig.Extensions.Xmd.CSCode;
using Markdig.Helpers;

namespace Markdig.Extensions.Xmd;

public static class XmdLanguageExtensions
{
    public static MarkdownPipelineBuilder UseXmdLanguage(this MarkdownPipelineBuilder pipeline, CSCodeOptions options)
    {
        pipeline.Extensions.Add(new XmdLanguageExtension(options));
        return pipeline;
    }
    internal static void MoveForward(this StringSlice slice, int v)
    {
        for (int i = 0; i < v; i++)
        {
            slice.NextChar();
        }
    }

}
