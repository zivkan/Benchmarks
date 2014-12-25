using System;

namespace LocalisedResourceExtractionBenchmark.Benchmark
{
    class Result
    {
        public Result(string name, double timeToFirst, double timeToComplete)
        {
            TimeToComplete = timeToComplete;
            TimeToFirst = timeToFirst;
            Name = name;
        }

        public String Name { get; private set; }
        public Double TimeToFirst { get; private set; }
        public Double TimeToComplete { get; private set; }
    }
}