using System.Collections.Generic;

namespace LocalisedResourceExtractionBenchmark
{
    interface ISourceRepository
    {
        IEnumerable<SourceModel> GetData();
    }
}
