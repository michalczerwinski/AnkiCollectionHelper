using Newtonsoft.Json;

namespace AnkiCollectionHelper.Anki.DbModel
{
    public class ModelField
    {
        [JsonProperty("ord")]
        public long Order { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }        
    }
}