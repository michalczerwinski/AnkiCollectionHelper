using System;
using System.Text;
using HtmlAgilityPack;

namespace AnkiCollectionHelper.Helpers
{
    public static class HtmlAgilityHelper
    {
        public static string ExtractText(this HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                return node.InnerText.Replace("&nbsp;", " ").Replace("&amp;", "&").Replace("&pound;", "£");
            }

            if (node.NodeType == HtmlNodeType.Element && node.Name == "br")
            {
                return Environment.NewLine;
            }

            var result = new StringBuilder();

            foreach (var childNode in node.ChildNodes)
            {
                var text = childNode.ExtractText();
                result.Append(text);
            }

            return result.ToString();
        }

        public static string ExtractText(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return ExtractText(htmlDocument.DocumentNode);
        }
    }
}