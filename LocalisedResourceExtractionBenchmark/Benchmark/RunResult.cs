using System;

namespace LocalisedResourceExtractionBenchmark.Benchmark
{
    class RunResult
    {
        public int Rows { get; private set; }
        public TimeSpan TimeToFirst { get; private set; }
        public TimeSpan TimeToComplete { get; private set; }

        public RunResult(int rows, TimeSpan timeToFirst, TimeSpan timeToComplete)
        {
            Rows = rows;
            TimeToComplete = timeToComplete;
            TimeToFirst = timeToFirst;
        }
    }
}
