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
            DatabaseTests.CreateTablesAndData();

            //var results = BenchmarkRunner.Run<DatabaseTests>(new AllowNonOptimized());
            BenchmarkRunner.Run<DatabaseTests>();

            //ReportWriter.UpdateReadme(results);
        }

        public class AllowNonOptimized : ManualConfig
        {
            public AllowNonOptimized()
            {
                AddValidator(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

                AddLogger(DefaultConfig.Instance.GetLoggers().ToArray()); // manual config has no loggers by default
                AddExporter(DefaultConfig.Instance.GetExporters().ToArray()); // manual config has no exporters by default
                AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray()); // manual config has no columns by default
            }
        }
    }
}
