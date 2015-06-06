using AnkiCollectionHelper.SqlLite;

namespace AnkiCollectionHelper.Anki.DbModel
{
    [Table("col")]
    public class Decks
    {
        [PrimaryKey]
        [Column("id")]
        public long Id { get; set; }

        [Column("models")]
        public string ModelsBlob { get; set; }

        [Column("mod")]
        public long LastModifiedOn { get; set; }
    }
}