using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.Extensions.Xmd
{
    public class HtmlReportObject
    {
        public string Html { get; set; }
    }

    public class MarkdownReportObject
    {
        public string Markdown { get; set; }
    }

    public class XmdDocument
    {
        private static readonly Lazy<XmdDocument> lazy = new Lazy<XmdDocument>(() => new XmdDocument());

        public static XmdDocument Instance { get { return lazy.Value; } }

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
}
