using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace AnkiCollectionHelper.Helpers
{
    public static class Extractor
    {
        public static string GetPronunciation(string word)
        {
            const string command = "http://www.macmillandictionary.com/dictionary/british/{0}";

            const string before = "<span class=\"SEP\" context=\"PRON-before\"> \u002F</span>";
            const string after = @"<span class=""SEP"" context=""PRON-after"">/</span>";

            word = word.Trim().ToLower();
            string url = String.Format(command, word);

            try
            {
                return WebHelper.ExtractTextFromHtmlPage(url, before, after);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string DownloadPronunciation(string word)
        {
            try
            {
                const string textBeforeSoundSource = " data-src-mp3=\"";
                const string textAfterSoundSource = "\"";

                word = word.Trim().ToLower();
                string url = String.Format("http://www.macmillandictionary.com/dictionary/british/{0}", word);
                string soundFileUrl = WebHelper.ExtractTextFromHtmlPage(url, textBeforeSoundSource, textAfterSoundSource, "Mozilla", 8000);

                if (soundFileUrl == null)
                {
                    return null;
                }

                var outputFileName = Path.GetTempFileName();
                WebHelper.DownloadToFile(soundFileUrl, outputFileName);
                return outputFileName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetSentencesFromTatoeba(string word, string fromLanguage = "eng", string toLanguage = "pol")
        {
            NormalizeEnglishWord(ref word);
            var url = string.Format("http://tatoeba.org/pol/sentences/search?query={0}&from={1}&to={2}", word, fromLanguage, toLanguage);
            string result = WebHelper.ExtractDataFromWebStream(url, ExtractTatoebaSentences, Encoding.UTF8);

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public static string GetSentenceTranslationForTatoeba(string sentence, string originalLanguage = "eng", string toLanguage = "spa")
        {
            sentence = sentence.Remove(".", ".", ":", "!");
            string[] allSentences = GetSentencesFromTatoeba(sentence, originalLanguage, toLanguage).Split('|');

            if (allSentences.Length == 0)
            {
                return "[NoTranslation]";
            }

            var translations = allSentences.Skip(1).ToArray();
            var separator = translations.Length > 1 ? " && " : "";
            return string.Join(separator, translations);
        }

        public static string GetSentencesTranslation(string sentences, string originalLanguage = "eng", string toLanguage = "spa")
        {
            return string.Join("|", sentences.Split('|').Select(s => GetSentenceTranslationForTatoeba(s, originalLanguage, toLanguage)));
        }

        private static string ExtractTatoebaSentences(HtmlDocument document)
        {
            var nodes = document.DocumentNode.SelectNodes("//div[@class=\"sentences_set\"]");

            if (nodes == null)
            {
                return string.Empty;
            }

            var result = string.Join("||", nodes.Select(n => string.Join("|", n.SelectNodes(".//div[@class=\"sentenceContent\"]").Select(m => m.InnerText.Trim()))));
            return result;
        }

        public static string GetDefinitionFromMegaslownik(string word)
        {
            NormalizeEnglishWord(ref word);
            string result = WebHelper.ExtractDataFromWebStream("http://megaslownik.pl/slownik/angielsko_polski/" + word, document => HtmlDocumentHelper.ExtractTextByXPath(document, "//div[@class=\"definicja\"]", "|"), Encoding.UTF8);

            if (result == null)
            {
                return null;
            }

            return result.Remove("&raquo;").Replace("  ", " ").Replace("\t\t", "\t");
        }

        public static string GetPronunciationFromWiktionary(string word)
        {
            NormalizeEnglishWord(ref word);
            var content = WebHelper.ExtractDataFromXmlStream(@"http://pl.wiktionary.org/w/api.php?format=xml&action=query&prop=revisions&rvprop=content&titles=" + word, ExtractWictionaryContent);
            var pronunciation = content.GetTextBetween("{{IPA|", "}}");

            if (string.IsNullOrEmpty(pronunciation))
            {
                return null;
            }

            return string.Format("/{0}/", pronunciation);
        }

        public static string GetPhraseologyFromWiktionary(string word)
        {
            NormalizeEnglishWord(ref word);
            var content = WebHelper.ExtractDataFromXmlStream(@"http://pl.wiktionary.org/w/api.php?format=xml&action=query&prop=revisions&rvprop=content&titles=" + word, ExtractWictionaryContent);
            return content.GetTextBetween("{{frazeologia}}", "{{etymologia}}").RemoveWikiTags().Replace(" • ", "|");
        }

        public static string GetSamplesFromYourDictionary(string word)
        {
            NormalizeEnglishWord(ref word);
            string result = WebHelper.ExtractDataFromWebStream("http://sentence.yourdictionary.com/" + word, document => HtmlDocumentHelper.ExtractTextByXPath(document, "//ul[@class=\"example\"]/li", "|"));
            return result.Remove("\n", "\r");
        }

        public static string GetBritishPronunciationFromMacmillan(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://www.macmillandictionary.com/dictionary/british/" + word,
                ExtractPronunciationIpaFromMacmillan);
        }

        public static string GetPopularityFromMacmillan(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://www.macmillandictionary.com/dictionary/british/" + word,
                delegate(HtmlDocument document)
                {
                    var stars = document.DocumentNode.SelectNodes("//img").Where(n => n.Attributes["src"].Value.Contains("star.gif"));
                    return stars.Count().ToString(CultureInfo.InvariantCulture);
                }
                );
        }

        public static string GetAmericanPronunciationFromMacmillan(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://www.macmillandictionary.com/dictionary/american/" + word,
                ExtractPronunciationIpaFromMacmillan);
        }

        public static string GetBritishPronunciationUrlFromMacmillan(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://www.macmillandictionary.com/dictionary/british/" + word,
                ExtractPronunciationUrlFromMacmillan);
        }

        public static string GetAmericanPronunciationUrlFromMacmillan(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://www.macmillandictionary.com/dictionary/american/" + word, ExtractPronunciationUrlFromMacmillan);
        }

        public static string GetIdiomFromFreeDictionary(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://www.thefreedictionary.com/" + word,
                document => HtmlDocumentHelper.ExtractTextByXPath(document, "//div[@class=\"idmseg\"]", "|"));
        }

        public static string GetIdomsFromCambridgeLearnerDictionary(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://dictionary.cambridge.org/search/learner-english/?type=idiom&q=" + word,
                document => HtmlDocumentHelper.ExtractTextByXPath(document, "//span[@class=\"phrase\"]", "|"));
        }

        public static string GetDefinitionsFromCambridgeLearnerDictionary(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://dictionary.cambridge.org/dictionary/learner-english/" + word,
                document => HtmlDocumentHelper.ExtractTextByXPath(document, "//span[@class=\"def\"]", "|"));
        }

        public static string GetSamplesFromCambridgeLearnerDictionary(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://dictionary.cambridge.org/dictionary/learner-english/" + word,
                document => HtmlDocumentHelper.ExtractTextByXPath(document, "//span[@class=\"eg\"]", "|"));
        }

        public static string GetSamplesFromDictionaryCom(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://dictionary.cambridge.org/dictionary/learner-english/" + word,
                document => HtmlDocumentHelper.ExtractTextByXPath(document, "//span[@class=\"eg\"]", "|"));
        }

        public static string GetPhrasalVerbsFromCambridgeLearnerDictionary(string word)
        {
            NormalizeEnglishWord(ref word);
            return WebHelper.ExtractDataFromWebStream("http://dictionary.cambridge.org/search/learner-english/?type=pv&q=come" + word,
                document => HtmlDocumentHelper.ExtractTextByXPath(document, "//span[@class=\"phrase\"]", "|"));
        }

        private static string ExtractPronunciationUrlFromMacmillan(HtmlDocument htmlDocument)
        {
            var nodes = htmlDocument.DocumentNode.SelectNodes("//img[@src=\"http://www.macmillandictionary.com/external/images/redspeaker.gif\"]");

            return string.Join("|", nodes.Select(n =>
            {
                var result = n.Attributes["onclick"].Value.Replace("playSoundFromFlash('/', '", "http://www.macmillandictionary.com");
                result = result.Replace("', this)", string.Empty);
                return result;
            }));
        }

        private static string ExtractPronunciationIpaFromMacmillan(HtmlDocument document)
        {
            const string xPath = "//span[@class=\"PRON\"]";
            const string separator = " ";

            return HtmlDocumentHelper.ExtractTextByXPath(document, xPath, separator);
        }

        private static string ExtractWictionaryContent(XDocument xDocument)
        {
            var revNode = xDocument.DescendantNodes().Cast<XElement>().FirstOrDefault(e => e.Name == "rev");

            if (revNode == null)
            {
                return null;
            }

            var content = revNode.Value;

            var englishSectionStartPosition = content.IndexOf("({{jкzyk angielski}})", StringComparison.InvariantCultureIgnoreCase);
            var englishSectionEndPosition = content.IndexOf("== ", englishSectionStartPosition + 1, StringComparison.InvariantCultureIgnoreCase);

            if (englishSectionEndPosition == -1)
            {
                englishSectionEndPosition = content.Length - 1;
            }

            return content.Substring(englishSectionStartPosition, englishSectionEndPosition - englishSectionStartPosition);
        }

        private static void Normalize(ref string word)
        {
            if (string.IsNullOrEmpty(word)) return;
            word = word.ToLowerInvariant().Trim(new[] { ' ', '\t', '.', ',' });
        }

        private static void NormalizeEnglishWord(ref string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return;
            }

            Normalize(ref word);

            //skip article if any
            word = word.TryToRemovePrefix("a ").TryToRemovePrefix("an ").TryToRemovePrefix("the ");

            //regular normalization
            Normalize(ref word);
        }

        public static string ExtractPronunciationFromLingorado(string expression)
        {
            var browser = new ScrapingBrowser();

            var uri = new Uri("http://lingorado.com/ipa/", UriKind.Absolute);
            var webPage = browser.NavigateToPage(uri, HttpVerb.Get, string.Empty);
            var form = webPage.FindFormById("transcribe");
            form["text_to_transcribe"] = expression;
            form["submit"] = "Show transcription";
            form["output_dialect"] = "br";
            form["output_style"] = "only_tr";
            form["weak_forms"] = "on";
            form["preBracket"] = "";
            form["postBracket"] = "";
            
            var page = form.Submit();
            return string.Join(" ", page.Html.CssSelect(".transcribed_word").Select(n => n.InnerText));
        }
    }
}