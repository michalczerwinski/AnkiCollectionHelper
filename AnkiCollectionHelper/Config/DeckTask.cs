using System.Xml.Serialization;
using AnkiCollectionHelper.Anki.DbModel;
using AnkiCollectionHelper.Tasks;

namespace AnkiCollectionHelper.Config
{
    [XmlInclude(typeof(DownloadIpa))]
    [XmlInclude(typeof(DownloadPronunciation))]
    [XmlInclude(typeof(DownloadSampleSentences))]
    public abstract class DeckTask
    {
        public string TargetModelName { get; set; }

        public abstract bool Execute(Note note, string mediaFolder);
    }
}