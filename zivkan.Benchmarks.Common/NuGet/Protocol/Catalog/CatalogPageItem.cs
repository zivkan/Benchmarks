using System.Text.Json.Serialization;

namespace zivkan.Benchmarks.Common.NuGet.Protocol.Catalog
{
    public class CatalogPageItem
    {
        public string? id { get; set; }

        public string? type { get; set; }

        public string? commitId { get; set; }

        public string? commitTimeStamp { get; set; }

        [JsonPropertyName("nuget:id")]
        public string? packageId { get; set; }

        [JsonPropertyName("nuget:version")]
        public string? packageVersion { get; set; }
    }
}
