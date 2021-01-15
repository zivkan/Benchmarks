using System.Text.Json.Serialization;

namespace zivkan.Benchmarks.Common.NuGet.Protocol
{
    public class ServiceIndexResource
    {
        [JsonPropertyName("@id")]
        public string? Id { get; set; }

        [JsonPropertyName("@type")]
        public string? Type { get; set; }
    }
}
