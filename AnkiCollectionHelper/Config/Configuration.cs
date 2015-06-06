using System.Collections.Generic;

namespace AnkiCollectionHelper.Config
{
    public class Configuration
    {
        public string CollectionFile { get; set; }

        public List<DeckTask> Tasks { get; set; }

        public Configuration()
        {
            Tasks = new List<DeckTask>();
        }
    }
}
