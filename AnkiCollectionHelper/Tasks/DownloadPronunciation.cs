using System;
using System.IO;
using AnkiCollectionHelper.Anki.DbModel;
using AnkiCollectionHelper.Config;
using AnkiCollectionHelper.Helpers;

namespace AnkiCollectionHelper.Tasks
{
    public class DownloadPronunciation : DeckTask
    {
        public string ExpressionField { get; set; }
        public string SoundField { get; set; }

        public override bool Execute(Note note, string mediaFolder)
        {
            var expression = note.GetField(ExpressionField);
            var sound = HtmlAgilityHelper.ExtractText(note.GetField(SoundField));
            string normalizedName = expression.Remove("<", ">", "/");
            var fileName = Path.ChangeExtension(normalizedName, ".mp3");
            var destinationFullName = Path.Combine(mediaFolder, fileName);

            if (string.IsNullOrWhiteSpace(sound))
            {
                Console.Write("Downloading pronunciation for: " + expression);

                expression = EnglishHelper.GetNormalizedVocabulary(expression);
                var temporaryFile = Extractor.DownloadPronunciation(expression);

                if (temporaryFile != null)
                {
                    File.Move(temporaryFile, destinationFullName);
                    note.SetField(SoundField, "[sound:" + fileName + "]");
                    Console.WriteLine(" [OK]");
                    return true;
                }
                Console.WriteLine(" [Not found]");
            }

            return false;
        }
    }
}