using System;
using System.Linq;
using HtmlAgilityPack;

namespace AnkiCollectionHelper.Helpers
{
    public static class HtmlDocumentHelper
    {
        public static string ExtractTextByXPath(HtmlDocument document, string xPath, string separator)
        {
            var nodes = document.DocumentNode.SelectNodes(xPath);

            if (nodes == null || nodes.Count == 0)
            {
                return String.Empty;
            }

            return String.Join(separator, nodes.Select(n => n.InnerText.Trim()));
        }
    }
}