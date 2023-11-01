using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SemanticVersionBenchmarks.Implementations;
using SemanticVersionBenchmarks.Implementations.Parsers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SemanticVersionBenchmarks
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    public class ParseBenchmarks
    {
        private IReadOnlyList<string>? _versions;

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            _versions = await VersionData.GetVersionsAsync();
        }

        [Benchmark]
        public int SubString() => Parse(input => SubstringParser.Parse(input));

        [Benchmark(Baseline = true)]
        public int Span() => Parse(input => SpanParser.Parse(input));

        [Benchmark]
        public int Regex() => Parse(input => RegexParser.Parse(input));

        [Benchmark]
        public int StateMachine() => Parse(input => StateMachineParser.Parse(input));

        [Benchmark]
        public int NuGetVersion() => Parse(input => NuGet.Versioning.NuGetVersion.Parse(input));

        private int Parse(Action<string> parse)
        {
            if (_versions is null)
            {
                throw new InvalidOperationException("GlobalSetup not run");
            }

            int count = 0;

            var list = new List<VersionWithClassArray>(_versions.Count);
            for (int i = 0; i < _versions.Count; i++)
            {
                var input = _versions[i];
                parse(input);
                count++;
            }

            return count;
        }
    }
}
