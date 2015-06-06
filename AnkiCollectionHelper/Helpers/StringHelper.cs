using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AnkiCollectionHelper.Helpers
{
    public static class StringHelper
    {
        public static string FileToString(string filePath)
        {
            return FileToString(filePath, Encoding.UTF8);
        }

        public static string FileToString(string filePath, Encoding encoding)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                return StreamHelper.StreamToString(fileStream, encoding);
            }
        }

        public static void StringToFile(string content, string filePath)
        {
            StringToFile(content, filePath, Encoding.UTF8);
        }

        public static void StringToFile(string content, string filePath, Encoding encoding)
        {
            using (MemoryStream memoryStream = StreamHelper.StringToStream(content, encoding))
            {
                StreamHelper.WriteStreamToFile(memoryStream, filePath);
            }
        }

        public static string GetTextBetween(string orignalText, string before, string after)
        {
            int start = orignalText.IndexOf(before, StringComparison.Ordinal);
            int end = orignalText.IndexOf(after, start + before.Length, StringComparison.Ordinal);

            if (start != -1 && end != -1 && end > start)
            {
                var startIndex = start + before.Length;
                var length = end - start - before.Length;
                return orignalText.Substring(startIndex, length);
            }

            return null;
        }

        public static string RemoveFromText(string text, string toRemove)
        {
            int index = text.IndexOf(toRemove, StringComparison.Ordinal);

            if (index != -1)
            {
                text = text.Remove(index, toRemove.Length);
            }

            return text;
        }

        public static string XmlString(string s)
        {
            var builder = new StringBuilder(s);
            const string ampersandLabel = "&";
            const string ampLabel = "&amp;";
            builder.Replace(ampersandLabel, ampLabel);
            const string lessThanLabel = "<";
            const string ltLabel = "&lt;";
            builder.Replace(lessThanLabel, ltLabel);
            const string moreThanLabel = ">";
            const string gtLabel = "&gt;";
            builder.Replace(moreThanLabel, gtLabel);
            const string zeroLabel = "\0";
            builder.Replace(zeroLabel, String.Empty);

            return builder.ToString();
        }

        public static string PlainTextToHtml(string s)
        {
            string result = s.Replace("\r\n", "<br />");
            result = result.Replace((char)3, ' ');
            result = result.Replace("\x002", "<br />");
            result = result.Replace((char)1, ' ');
            result = result.Replace((char)0, ' ');
            return result;
        }

        public static string RemoveHtmlTags(string htmlText)
        {
            htmlText = htmlText.Replace("</P>", "\r\n");
            var objRegEx = new Regex("<[^>]*>");
            return objRegEx.Replace(htmlText, "");
        }

        private static string GetCharAsHexCode(char c)
        {
            const string hexFormat = "{0:x}";
            string hexadecimal = string.Format(hexFormat, (int)c);

            while (hexadecimal.Length < 4) hexadecimal = '0' + hexadecimal;
            return hexadecimal;
        }

        private static string GetCharAsRegexSymbol(char c)
        {
            const string regexUnicodeCharFormat = @"\u{0}";
            return string.Format(regexUnicodeCharFormat, GetCharAsHexCode(c));
        }

        public static string ToRegexpEscapedString(this string s)
        {
            var result = new StringBuilder(s.Length * 4);
            foreach (char c in s)
            {
                result.Append(GetCharAsRegexSymbol(c));
            }

            return result.ToString();
        }

        public static string TrimPrefix(this string s, string prefix)
        {
            if (s != null && s.StartsWith(prefix))
            {
                return s.Substring(prefix.Length);
            }

            return s;
        }
    }
}