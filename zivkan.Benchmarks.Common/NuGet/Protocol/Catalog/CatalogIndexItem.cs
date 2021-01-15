using System.Text.Json.Serialization;

namespace zivkan.Benchmarks.Common.NuGet.Protocol.Catalog
{
    public class CatalogIndexItem
    {
        [JsonPropertyName("@id")]
        public string? id { get; set; }

        public string? commitId { get; set; }

        public string? commitTimeStamp { get; set; }

        public int? count { get; set; }
    }
}
