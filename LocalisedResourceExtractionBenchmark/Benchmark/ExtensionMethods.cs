using System.Diagnostics;

namespace LocalisedResourceExtractionBenchmark.Benchmark
{
    static class Extensions
    {
        public static RunResult Run(this ISourceRepository repo)
        {
            var latencyTimer = new Stopwatch();
            var completeTimer = new Stopwatch();
            var enumerator = repo.GetData().GetEnumerator();
            int rows = 0;

            latencyTimer.Start();
            completeTimer.Start();
            if (enumerator.MoveNext())
                rows++;
            latencyTimer.Stop();

            for (; enumerator.MoveNext(); )
            {
                rows++;
            }
            completeTimer.Stop();

            return new RunResult(rows, latencyTimer.Elapsed, completeTimer.Elapsed);
        }
    }
}
