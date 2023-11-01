using BenchmarkDotNet.Reports;
using LocalisedResourceExtractionBenchmark.Extractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace LocalisedResourceExtractionBenchmark.Benchmark
{
    internal static class ReportWriter
    {
        public static void UpdateReadme(Summary results)
        {
            var filename = GetPathOfFileAbove("Readme.md", typeof(DatabaseTests).Assembly.Location);
            if (filename == null)
            {
                throw new Exception("Couldn't find Readme.md");
            }

            var contents = File.ReadAllLines(filename).ToList();
            var resultsIndex = contents.IndexOf("## <a id=\"results\">Results</a>");
            if (resultsIndex < 0)
                throw new Exception("Could not find results header");
            var countToRemove = contents.Count - resultsIndex - 1;
            contents.RemoveRange(resultsIndex + 1, countToRemove);

            var convertedResults = ConvertResults(results.Reports);
            var fastestResult = convertedResults.multipleLanguages.OrderBy(r => r.TimeToComplete).First();

            contents.Add("");
            contents.Add("|Extraction Method|Time to complete|Memory Allocated|GC collections per 1k runs (gen0 gen1 gen2)|");
            contents.Add("|-----------------|----------------|----------------|-------------------------------------------|");
            contents.Add(ResultString(convertedResults.singleLanguage, fastestResult));
            contents.AddRange(convertedResults.multipleLanguages.Select(result => ResultString(result, fastestResult)));

            File.WriteAllLines(filename, contents);
        }

        private static (Result singleLanguage, List<Result> multipleLanguages) ConvertResults(ImmutableArray<BenchmarkReport> reports)
        {
            var results = new List<Result>();
            Result singleLanguageResult = null;
            var singleLanguageType = typeof(SingleLanguage);

            foreach (var report in reports)
            {
                var type = (Type)report.BenchmarkCase.Parameters[0].Value;
                var time = report.ResultStatistics.Mean / 1_000_000;
                var memoryAllocated = report.GcStats.GetBytesAllocatedPerOperation(report.BenchmarkCase).Value / 1024m / 1024m;
                var gcScalingFactor = 1000m / report.GcStats.TotalOperations;
                var gcGen0 = report.GcStats.Gen0Collections * gcScalingFactor;
                var gcGen1 = report.GcStats.Gen1Collections * gcScalingFactor;
                var gcGen2 = report.GcStats.Gen2Collections * gcScalingFactor;
                var result = new Result(type.Name, time, memoryAllocated, gcGen0, gcGen1, gcGen2);
                if (type == singleLanguageType)
                {
                    if (singleLanguageResult != null)
                    {
                        throw new Exception("Got single language result more than once");
                    }
                    singleLanguageResult = result;
                }
                else
                {
                    results.Add(result);
                }
            }

            if (singleLanguageResult == null)
            {
                throw new Exception("Didn't find single language result");
            }

            results.Sort(Comparer<Result>.Create((a, b) => a.TimeToComplete.CompareTo(b.TimeToComplete)));

            return (singleLanguageResult, results);
        }

        private static string ResultString(Result result, Result fastestResult)
        {
            var completePct = result.TimeToComplete * 100.0 / fastestResult.TimeToComplete;
            return $"|" +
                $"{result.Name}|" +
                $"{(int)result.TimeToComplete:0} ms ({completePct:0}%)|" +
                $"{result.MemoryAllocated:0.0} MB|" +
                $"{(int)result.GcGen0Per1000} {(int)result.GcGen1Per1000} {(int)result.GcGen2Per1000}|";
        }

        private static string GetPathOfFileAbove(string fileName, string initialPath)
        {
            string lastPath = initialPath;
            string fullPath = Path.Combine(initialPath, fileName);

            while (!File.Exists(fullPath))
            {
                var tryPath = Path.GetDirectoryName(lastPath);
                if (string.Equals(lastPath, tryPath))
                {
                    return null;
                }

                fullPath = Path.Combine(tryPath, fileName);
                lastPath = tryPath;
            }

            return fullPath;
        }
    }
}
