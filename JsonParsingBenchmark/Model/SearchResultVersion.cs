using System.Text.Json.Serialization;

namespace JsonParsingBenchmark.Model
{
    public class SearchResultVersion
    {
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("downloads")]
        public int Downloads { get; set; }
    }
}
