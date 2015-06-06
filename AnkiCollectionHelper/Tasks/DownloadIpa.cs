using System;
using AnkiCollectionHelper.Anki.DbModel;
using AnkiCollectionHelper.Config;
using AnkiCollectionHelper.Helpers;

namespace AnkiCollectionHelper.Tasks
{
    public class DownloadIpa : DeckTask
    {
        public string ExpressionField { get; set; }
        public string IpaField { get; set; }

        public override bool Execute(Note note, string mediaFolder)
        {
            var expression = note.GetField(ExpressionField);
            var pronunciation = HtmlAgilityHelper.ExtractText(note.GetField(IpaField));
            Console.Write("Downloading IPA for: " + expression);

            if (string.IsNullOrWhiteSpace(pronunciation))
            {
                expression = EnglishHelper.GetNormalizedVocabulary(expression);
                pronunciation = Extractor.GetPronunciation(expression);

                if (string.IsNullOrWhiteSpace(pronunciation))
                {
                    pronunciation = Extractor.ExtractPronunciationFromLingorado(expression);
                }

                if (!string.IsNullOrWhiteSpace(pronunciation))
                {
                    note.SetField(IpaField, pronunciation);
                    Console.WriteLine(" [OK]");
                    return true;
                }
            }

            Console.WriteLine(" [Not found]");
            return false;
        }
    }
}