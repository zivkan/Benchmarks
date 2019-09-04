using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonParsingBenchmark.Model
{
    public class SearchResults
    {
        [JsonPropertyName("totalHits")]
        public int TotalHits { get; set; }

        [JsonPropertyName("data")]
        public List<SearchResult> Data { get; set; }
    }
}
