using System.Collections.Generic;

namespace zivkan.Benchmarks.Common.NuGet.Protocol.Catalog
{
    public class CatalogIndex
    {
        public int? count { get; set; }

        public List<CatalogIndexItem>? items { get; set; }
    }
}
