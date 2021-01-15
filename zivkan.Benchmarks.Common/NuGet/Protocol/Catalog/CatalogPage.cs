using System.Collections.Generic;

namespace zivkan.Benchmarks.Common.NuGet.Protocol.Catalog
{
    public class CatalogPage
    {
        public string? id { get; set; }

        public string? commitId { get; set; }

        public string? commitTimeStamp { get; set; }

        public int? count { get; set; }

        public List<CatalogPageItem>? items { get; set; }
    }
}
