using System;
using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;

namespace AnkiCollectionHelper.Helpers
{
    public class KindleHelper
    {
        private static ScrapingBrowser _browser;

        public static string GetHighlight(string password)
        {
            if (_browser == null)
            {
                _browser = new ScrapingBrowser
                {
                    IgnoreCookies = true
                };
                var loginUri = new Uri("https://kindle.amazon.com/login", UriKind.Absolute);
                var webPage = _browser.NavigateToPage(loginUri, HttpVerb.Get, string.Empty);

                var htmlNode = webPage.Html.CssSelect("#ap_signin_form").Single();
                var pageWebForm = new PageWebForm(htmlNode, _browser);
                pageWebForm["email"] = "michalczerwinski@gmail.com";
                pageWebForm["password"] = password;
                var loggedWebPage = pageWebForm.Submit();
            }

            var highlightsPage = _browser.NavigateToPage(new Uri("https://kindle.amazon.com/your_highlights", UriKind.Absolute), HttpVerb.Get, string.Empty);

            var node = highlightsPage.Html.CssSelect(".highlightRow").FirstOrDefault();

            if (node == null)
            {
                return null;
            }

            var word = node.CssSelect(".highlight").Single().InnerText;
            var deleteForm = new PageWebForm(node.CssSelect(".deleteHighlightForm").Single(), _browser);
            var res = deleteForm.Submit();
            return word;
        }
    }
}
