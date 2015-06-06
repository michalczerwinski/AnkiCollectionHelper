using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace AnkiCollectionHelper.Helpers
{
    public static class WebHelper
    {
        private static readonly Dictionary<string, HtmlDocument> DownloadedHtmlDocuments = new Dictionary<string, HtmlDocument>();
        private static readonly Dictionary<string, XDocument> DownloadedXDocuments = new Dictionary<string, XDocument>();

        public static string GetPageAsString(string url, int timeOut)
        {
            return GetPageAsString(url, timeOut, null);
        }

        public static string GetPageAsString(string url, int timeOut, string userAgent)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeOut;
            request.UserAgent = userAgent;
            var response = (HttpWebResponse)request.GetResponse();
            Encoding encoding = response.CharacterSet != null
                                    ? Encoding.GetEncoding(response.CharacterSet)
                                    : Encoding.UTF8;
            return StreamHelper.StreamToString(response.GetResponseStream(), encoding);
        }

        public static string ExtractTextFromHtmlPage(string url, string before, string after)
        {
            return ExtractTextFromHtmlPage(url, before, after, null, 5000);
        }

        public static string ExtractTextFromHtmlPage(string url, string before, string after, string userAgent, int timeOut)
        {
            string pageAsString = GetPageAsString(url, timeOut, userAgent);
            return StringHelper.GetTextBetween(pageAsString, before, after);
        }

        public static void DownloadToFile(string url, string outputFileName)
        {
            Stream stream = DownloadToStream(url);
            StreamHelper.WriteStreamToFile(stream, outputFileName);
        }

        public static Stream DownloadToStream(string url)
        {
            WebRequest request = WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }

        public static Image ImageFromUrl(string url)
        {
            return Image.FromStream(DownloadToStream(url));
        }

        public static T ExtractDataFromWebStream<T>(string url, Func<HtmlDocument, T> extractFunction, Encoding encodingToEnforce = null) where T : class
        {
            HtmlDocument htmlDocument;
            bool isInCache;

            lock (DownloadedHtmlDocuments)
            {
                isInCache = DownloadedHtmlDocuments.TryGetValue(url, out htmlDocument);
            }

            if (isInCache)
            {
                return extractFunction(htmlDocument);
            }

            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    htmlDocument = new HtmlDocument();
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        var encoding = encodingToEnforce ?? (webResponse.CharacterSet != null
                                                                 ? Encoding.GetEncoding(webResponse.CharacterSet)
                                                                 : Encoding.UTF8);
                        htmlDocument.Load(responseStream, encoding);

                        lock (DownloadedHtmlDocuments)
                        {
                            if (DownloadedHtmlDocuments.Count > 300)
                            {
                                DownloadedHtmlDocuments.Clear();
                            }

                            DownloadedHtmlDocuments[url] = htmlDocument;
                        }

                        return extractFunction(htmlDocument);
                    }
                }
            }
            catch (WebException)
            {
                return null;
            }
        }

        public static T ExtractDataFromXmlStream<T>(string url, Func<XDocument, T> extractFunction) where T : class
        {
            XDocument xDocument;
            bool isInCache;

            lock (DownloadedXDocuments)
            {
                isInCache = DownloadedXDocuments.TryGetValue(url, out xDocument);
            }

            if (isInCache)
            {
                return extractFunction(xDocument);
            }

            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.UserAgent = "Memonica (http://Memonica.com/Memonica/)";
                using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        xDocument = XDocument.Load(responseStream);

                        lock (DownloadedXDocuments)
                        {
                            DownloadedXDocuments[url] = xDocument;
                        }

                        return extractFunction(xDocument);
                    }
                }
            }
            catch (WebException)
            {
                return null;
            }
        }
    }

}
