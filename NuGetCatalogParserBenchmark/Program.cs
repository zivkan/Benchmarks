using BenchmarkDotNet.Running;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace NuGetCatalogParserBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var cachePath = Utility.GetCachePath();
            InitialiseAsync(cachePath).GetAwaiter().GetResult();

            var result = BenchmarkRunner.Run<Benchmarks>();
        }

        private static async Task InitialiseAsync(string cachePath)
        {
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var mirror = new Mirror(cachePath);
            var sw = Stopwatch.StartNew();
            await mirror.Run(true);
            sw.Stop();
            Console.WriteLine("Checked {0} downloaded {1} after {2}", mirror.Checked, mirror.Downlaoded, sw.Elapsed);
        }
    }
}
