using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace AnkiCollectionHelper.Helpers
{
    public static class StringHelper2
    {
        /// <summary>
        /// The method create a Base64 encoded string from a normal string.
        /// </summary>
        /// <param name="strToEncode">The String containing the characters to encode.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string EncodeTo64(this string strToEncode)
        {
            var toEncodeAsBytes = Encoding.UTF8.GetBytes(strToEncode);

            return Convert.ToBase64String(toEncodeAsBytes);
        }

        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        public static string SanitizeXmlString(this string xml)
        {
            if (xml == null)
            {
                return null;
            }

            var buffer = new StringBuilder(xml.Length);

            foreach (var c in xml.Where(c => IsLegalXmlChar(c)))
            {
                buffer.Append(c);
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        private static bool IsLegalXmlChar(int character)
        {
            return
                (
                    character == 0x9 /* == '\t' == 9   */||
                    character == 0xA /* == '\n' == 10  */||
                    character == 0xD /* == '\r' == 13  */||
                    (character >= 0x20 && character <= 0xD7FF) ||
                    (character >= 0xE000 && character <= 0xFFFD) ||
                    (character >= 0x10000 && character <= 0x10FFFF)
                );
        }

        public static string XmlEscape(this string unescaped)
        {
            var doc = new XmlDocument();
            var node = doc.CreateElement("root");
            node.InnerText = unescaped;
            var result = node.InnerXml;
            result = result.Replace("\"", "&quot;");
            result = result.Replace("'", "&apos;");
            return result;
        }

        public static string XmlUnescape(this string escaped)
        {
            var doc = new XmlDocument();
            var node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return node.InnerText;
        }

        public static string RemovePrefix(this string s, string prefix)
        {
            if (s.StartsWith(prefix))
            {
                return s.Substring(prefix.Length);
            }

            throw new InvalidDataException("String doesn't start with a given prefix");
        }

        public static string TryToRemovePrefix(this string s, string prefix)
        {
            if (s.StartsWith(prefix))
            {
                return s.Substring(prefix.Length);
            }

            return s;
        }

        public static string GetTextBetween(this string s, string before, string after)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            var startIndex = s.IndexOf(before, StringComparison.Ordinal);

            if (startIndex == -1)
            {
                return null;
            }

            startIndex += before.Length;
            var endIndex = s.IndexOf(after, startIndex, StringComparison.Ordinal);

            return s.Substring(startIndex, endIndex - startIndex);
        }

        public static string Remove(this string s, string textToRemove)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            return s.Replace(textToRemove, string.Empty);
        }

        public static string Remove(this string s, params string[] textsToRemove)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            return textsToRemove.Aggregate(s, (current, textToRemove) => current.Remove(textToRemove));
        }

        public static string RemoveWikiTags(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            return s.Remove("{{", "}}", "[[", "]]", "''").Trim('\n', '\r', ' ', '\t');
        }
    }
}