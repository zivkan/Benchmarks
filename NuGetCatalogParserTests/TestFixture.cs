using NuGetCatalogParserBenchmark;
using NuGetCatalogParserBenchmark.Merge;
using System;
using System.Collections.Generic;

namespace NuGetCatalogParserTests
{
    public class TestFixture
    {
        public TestFixture()
        {
            CachePath = Utility.GetCachePath();

            var mirror = new Mirror(CachePath);
            mirror.Run(false).GetAwaiter().GetResult();

            var results = new LockFreeOffloadedSingleThreadedMerge(CachePath, Environment.ProcessorCount).Go().GetAwaiter().GetResult();
            ExpectedResult = results;
        }

        public string CachePath { get; }

        public Dictionary<string, List<string>> ExpectedResult { get; }
    }
}
