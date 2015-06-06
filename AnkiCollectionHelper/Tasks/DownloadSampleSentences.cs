using System;
using AnkiCollectionHelper.Anki.DbModel;
using AnkiCollectionHelper.Config;
using AnkiCollectionHelper.Helpers;

namespace AnkiCollectionHelper.Tasks
{
    public class DownloadSampleSentences : DeckTask
    {
        public string ExpressionField { get; set; }
        public string SamplesField { get; set; }

        public override bool Execute(Note note, string mediaFolder)
        {
            var expression = note.GetField(ExpressionField);
            var samples = HtmlAgilityHelper.ExtractText(note.GetField(SamplesField));

            if (!string.IsNullOrWhiteSpace(samples))
            {
                return false;
            }

            expression = EnglishHelper.GetNormalizedVocabulary(expression);
            Console.Write("Downloading samples for: " + expression);
            samples = Extractor.GetSamplesFromCambridgeLearnerDictionary(expression);

            if (string.IsNullOrWhiteSpace(samples))
            {
                samples = Extractor.GetSamplesFromDictionaryCom(expression);
            }

            if (string.IsNullOrWhiteSpace(samples))
            {
                samples = Extractor.GetSamplesFromYourDictionary(expression);
            }

            if (!string.IsNullOrWhiteSpace(samples))
            {
                note.SetField(SamplesField, samples.Replace("|", "<br/>"));
                Console.WriteLine(" [OK]");
                return true;
            }

            Console.WriteLine(" [Not found]");
            return false;
        }
    }
}