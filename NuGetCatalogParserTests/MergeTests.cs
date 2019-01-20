using NuGetCatalogParserBenchmark.Merge;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NuGetCatalogParserTests
{
    public class MergeTests : IClassFixture<TestFixture>
    {
        private TestFixture _fixture;

        public MergeTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestSingleThreadedMerge()
        {
            var test = new SingleThreadedMerge(_fixture.CachePath);
            var result = test.Go();
            ResultComparer.Compare(_fixture.ExpectedResult, result);
        }

        [Fact]
        public async Task TestConcurrentDictionaryMerge()
        {
            var test = new ConcurrentDictionaryMerge(_fixture.CachePath);
            var result = await test.Go();
            ResultComparer.Compare(_fixture.ExpectedResult, result);
        }

        [Fact]
        public async Task TestLockFreeOffsetSingleThreadedMerge()
        {
            var test = new LockFreeOffloadedSingleThreadedMerge(_fixture.CachePath, Environment.ProcessorCount);
            var result = await test.Go();
            ResultComparer.Compare(_fixture.ExpectedResult, result);
        }

        [Fact]
        public void TestMultiThreadedIndependant()
        {
            var test = new MultiThreadedMerge(_fixture.CachePath, Environment.ProcessorCount);
            var result = test.Go();
            ResultComparer.Compare(_fixture.ExpectedResult, result);
        }
    }
}
