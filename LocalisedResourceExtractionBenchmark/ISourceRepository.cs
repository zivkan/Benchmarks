using System.Collections.Generic;
using System.Data;

namespace LocalisedResourceExtractionBenchmark
{
    interface ISourceRepository
    {
        IEnumerable<SourceModel> GetData();
    }
}
