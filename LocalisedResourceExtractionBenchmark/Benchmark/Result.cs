using System;

namespace LocalisedResourceExtractionBenchmark.Benchmark
{
    class Result
    {
        public Result(string name, double timeToComplete, decimal memoryAllocated, decimal gcGen0Per1000, decimal gcGen1Per1000, decimal gcGen2Per1000)
        {
            TimeToComplete = timeToComplete;
            Name = name;
            MemoryAllocated = memoryAllocated;
            GcGen0Per1000 = gcGen0Per1000;
            GcGen1Per1000 = gcGen1Per1000;
            GcGen2Per1000 = gcGen2Per1000;
        }

        public string Name { get; }
        public double TimeToComplete { get; }
        public decimal MemoryAllocated { get; }
        public decimal GcGen0Per1000 { get; }
        public decimal GcGen1Per1000 { get; }
        public decimal GcGen2Per1000 { get; }
    }
}