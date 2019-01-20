using Newtonsoft.Json;
using System.Collections.Generic;

namespace NuGetCatalogParserBenchmark.NuGetModel
{
    public class CatalogIndex
    {
        public int Count { get; set; }

        public List<Item> Items { get; set; }

        public class Item
        {
            [JsonProperty("@id")]
            public string Id { get; set; }

            public string CommitId { get; set; }

            public string CommitTimeStamp { get; set; }

            public int Count { get; set; }
        }
    }
}
