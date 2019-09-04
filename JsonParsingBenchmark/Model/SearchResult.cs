using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonParsingBenchmark.Model
{
    public class SearchResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("versions")]
        public List<SearchResultVersion> Versions { get; set; }

        [JsonPropertyName("authors")]
        public List<string> Authors { get; set; }

        [JsonPropertyName("iconUrl")]
        public string IconUrl { get; set; }

        [JsonPropertyName("licenseUrl")]
        public string LicenseUrl { get; set; }

        [JsonPropertyName("owners")]
        public List<string> Owners { get; set; }

        [JsonPropertyName("projectUrl")]
        public string ProjectUrl { get; set; }

        [JsonPropertyName("registration")]
        public string Registration { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("totalDownloads")]
        public int TotalDownloads { get; set; }

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }
    }
}