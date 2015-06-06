using System.Globalization;
using AnkiCollectionHelper.SqlLite;

namespace AnkiCollectionHelper.Anki.DbModel
{
    [Table("notes")]
    public class Note
    {
        private const char Separator = '\x1F';

        [PrimaryKey]
        [Column("id")]
        public long Id { get; set; }

        [Column("mid")]
        public long ModelId { get; set; }

        [Column("usn")]
        public long SynchronizationCounter { get; set; }

        [Column("mod")]
        public long ModifiedOnEpochSeconds { get; set; }

        [Column("flds")]
        public string Fields { get; set; }

        [Ignore]
        public DeckModel Model { get; set; }

        public string GetField(string name)
        {
            var index = Model.GetFieldIndex(name);
            var fields = Fields.Split(Separator);
            return fields[(int)index];
        }

        public void SetField(string name, string value)
        {
            var index = Model.GetFieldIndex(name);
            var fields = Fields.Split(Separator);
            fields[(int) index] = value;
            Fields = string.Join(Separator.ToString(CultureInfo.InvariantCulture), fields);
        }
    }
}