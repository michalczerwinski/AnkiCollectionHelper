using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnkiCollectionHelper.Anki;
using AnkiCollectionHelper.Config;
using AnkiCollectionHelper.Helpers;

namespace AnkiCollectionHelper
{
    public class Program
    {
        private const string ProcessedNotesFileName = "processed.txt";

        public static void Main()
        {
            var processedNotes = GetProcessedNotesHashSet();
            var configuration = XmlSerializationHelper.DeserializeFromFile<Configuration>("configuration.xml");

            using (var collection = new Collection(configuration.CollectionFile))
            {
                foreach (var note in collection.Notes)
                {
                    if (!processedNotes.Contains(note.Fields))
                    {
                        var currentNoteModelName = note.Model.Name;

                        // ReSharper disable once LoopCanBeConvertedToQuery
                        var deckTasks = configuration.Tasks
                            .Where(deckTask => deckTask.TargetModelName == currentNoteModelName)
                            .ToList();

                        bool wasChanged = deckTasks
                            .Aggregate(false, 
                                (current, task) => current | 
                                    task.Execute(note, collection.MediaFolder));

                        processedNotes.Add(note.Fields);

                        if (wasChanged)
                        {
                            collection.UpdateNote(note);
                        }
                    }
                }

                collection.Commit();
                File.WriteAllLines(ProcessedNotesFileName, processedNotes);
            }

            Console.WriteLine("Press any key to finish");
            Console.ReadKey(false);
        }

        private static HashSet<string> GetProcessedNotesHashSet()
        {
            File.AppendAllLines(ProcessedNotesFileName, new string[] {});
            var processedNotes = new HashSet<string>(File.ReadAllLines(ProcessedNotesFileName));
            return processedNotes;
        }
    }
}
