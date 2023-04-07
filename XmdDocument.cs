using System;

namespace Markdig.Extensions.Xmd;

public class XmdDocument
{
    private static readonly Lazy<XmdDocument> lazy = new(() => new XmdDocument());

    public static XmdDocument Instance { get { return lazy.Value; } }

#nullable enable
    /// <summary>
    /// Gets the current uri of the request, that invoked this instance
    /// </summary>
    public Uri? CurrentUri { get; internal set; } = null;
#nullable restore

    private XmdDocument() { }

    public object ReportObject { get; private set; }

    public void InsertHtml(string html)
    {
        ReportObject = new HtmlReportObject() { Html = html };
    }

    public void InsertMarkdown(string markdown)
    {
        ReportObject = new MarkdownReportObject() { Markdown = markdown };
    }

    public void Reset()
    {
        ReportObject = null;
    }
}