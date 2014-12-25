using System.Collections.Generic;
using System.Linq;
using LocalisedResourceExtractionBenchmark.Extractions;

namespace LocalisedResourceExtractionBenchmark.Benchmark
{
    class BenchmarkRunner
    {
        private readonly ISourceRepository[] _extractors;
        private readonly SingleLanguage _singleLanguage;
        private readonly int _runs;

        public BenchmarkRunner(ISourceRepository[] extractors, SingleLanguage singleLanguage, int runs)
        {
            _extractors = extractors;
            _singleLanguage = singleLanguage;
            _runs = runs;
        }

        public Results Run()
        {
            var single = new Tally(_singleLanguage);
            var extractors = _extractors.Select(e => new Tally(e)).ToList();

            for (int run = 0; run < _runs; run++)
            {
                var runResults = single.Extractor.Run();
                single.TimeToFirst.Add(runResults.TimeToFirst);
                single.TimeToComplete.Add(runResults.TimeToComplete);

                foreach (var extractor in extractors)
                {
                    runResults = extractor.Extractor.Run();
                    extractor.TimeToFirst.Add(runResults.TimeToFirst);
                    extractor.TimeToComplete.Add(runResults.TimeToComplete);
                }
            }

            var results = extractors.Select(e => new Result(e.Extractor.GetType().Name,
                e.TimeToFirst.Average(t => t.TotalSeconds), e.TimeToComplete.Average(t => t.TotalSeconds))).ToList();
            results.Sort((l,r) => l.TimeToComplete.CompareTo(r.TimeToComplete));
            var singleResult = new Result(_singleLanguage.GetType().Name,
                single.TimeToFirst.Average(t => t.TotalSeconds), single.TimeToComplete.Average(t => t.TotalSeconds));

            return new Results(singleResult, results);
        }
    }
}
