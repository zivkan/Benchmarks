using Newtonsoft.Json;
using System.Collections.Generic;

namespace NuGetCatalogParserBenchmark.NuGetModel
{
    public class ServiceIndex
    {
        public List<Resource> Resources { get; set; }

        public class Resource
        {
            [JsonProperty("@id")]
            public string Id { get; set; }

            [JsonProperty("@type")]
            public string Type { get; set; }
        }
    }
}
