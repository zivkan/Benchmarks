using Newtonsoft.Json;
using System.Collections.Generic;

namespace NuGetCatalogParserBenchmark.NuGetModel
{
    internal class CatalogPage
    {
        public string Id { get; set; }
        public string CommitId { get; set; }
        public string CommitTimeStamp { get; set; }
        public int Count { get; set; }
        public List<Item> Items { get; set; }

        internal class Item
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string CommitId { get; set; }
            public string CommitTimeStamp { get; set; }
            [JsonProperty("nuget:id")]
            public string PackageId { get; set; }
            [JsonProperty("nuget:version")]
            public string PackageVersion { get; set; }
        }
    }
}
