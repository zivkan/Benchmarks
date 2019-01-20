using BenchmarkDotNet.Attributes;
using NuGetCatalogParserBenchmark.Merge;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuGetCatalogParserBenchmark
{
    public class Benchmarks
    {
        private readonly string _cachePath;

        public Benchmarks()
        {
            _cachePath = Utility.GetCachePath();
        }

        [Benchmark]
        public Dictionary<string, List<string>> SingleThreadedMerge()
        {
            var test = new SingleThreadedMerge(_cachePath);
            return test.Go();
        }

        [Benchmark]
        public Task<Dictionary<string, List<string>>> ConcurrentDictionaryMerge()
        {
            var test = new ConcurrentDictionaryMerge(_cachePath);
            return test.Go();
        }

        [Benchmark]
        public Task<Dictionary<string, List<string>>> LockFreeOffloadedSingleThreadedMerge()
        {
            var test = new LockFreeOffloadedSingleThreadedMerge(_cachePath, Environment.ProcessorCount);
            return test.Go();
        }

        [Benchmark]
        public Dictionary<string, List<string>> TestMultiThreaded()
        {
            var test = new MultiThreadedMerge(_cachePath, Environment.ProcessorCount);
            return test.Go();
        }
    }
}
