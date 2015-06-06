using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AnkiCollectionHelper.Anki.DbModel
{
    public class DeckModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("flds")]
        public List<ModelField> Fields { get; set; }

        public long GetFieldIndex(string fieldName)
        {
            return Fields.Single(f => f.Name == fieldName).Order;
        }
    }
}