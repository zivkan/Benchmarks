using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using LocalisedResourceExtractionBenchmark.Benchmark;
using System.Linq;

namespace LocalisedResourceExtractionBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            // Allow non optimised only to debug the report writer. Don't use for comparing results.
            var results = BenchmarkRunner.Run<DatabaseTests>(new AllowNonOptimized());

            ReportWriter.UpdateReadme(results);
        }

        public class AllowNonOptimized : ManualConfig
        {
            public AllowNonOptimized()
            {
                Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

                Add(DefaultConfig.Instance.GetLoggers().ToArray()); // manual config has no loggers by default
                Add(DefaultConfig.Instance.GetExporters().ToArray()); // manual config has no exporters by default
                Add(DefaultConfig.Instance.GetColumnProviders().ToArray()); // manual config has no columns by default
            }
        }
    }
}
