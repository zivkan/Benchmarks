using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LocalisedResourceExtractionBenchmark
{
    public class SourceModel
    {
        public string Code { get; private set; }
        public string Parent { get; private set; }
        public IReadOnlyDictionary<string, string> Labels { get; private set; }

        public SourceModel(string code, string parent, IDictionary<string,string> labels)
        {
            Code = code;
            Parent = parent;
            Labels = new ReadOnlyDictionary<string, string>(labels);
        }
    }
}
