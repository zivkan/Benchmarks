using System;
using System.Collections.Generic;

namespace LocalisedResourceExtractionBenchmark.Benchmark
{
    class Tally
    {
        public ISourceRepository Extractor { get; private set; }
        public List<TimeSpan> TimeToFirst { get; private set; }
        public List<TimeSpan> TimeToComplete { get; private set; }

        public Tally(ISourceRepository extractor)
        {
            Extractor = extractor;
            TimeToFirst= new List<TimeSpan>();
            TimeToComplete = new List<TimeSpan>();
        }
    }
}