using Markdig.Extensions.Xmd.CSCode;
using Markdig.Helpers;
using System;

namespace Markdig.Extensions.Xmd;

public static class XmdLanguageExtensions
{
    public static MarkdownPipelineBuilder UseXmdLanguage(this MarkdownPipelineBuilder pipeline, CSCodeOptions options, Uri currentUri)
    {
        pipeline.Extensions.Add(new XmdLanguageExtension(options, currentUri));
        return pipeline;
    }

    internal static void MoveForward(this StringSlice slice, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            slice.NextChar();
        }
    }

}
