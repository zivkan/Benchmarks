using System.Collections.Generic;

namespace LocalisedResourceExtractionBenchmark.Benchmark
{
    class Results
    {
        public Results(Result singleLanguageResult, IReadOnlyList<Result> extractionResults)
        {
            ExtractionResults = extractionResults;
            SingleLanguageResult = singleLanguageResult;
        }

        public Result SingleLanguageResult { get; private set; }
        public IReadOnlyList<Result> ExtractionResults { get; private set; }
    }
}