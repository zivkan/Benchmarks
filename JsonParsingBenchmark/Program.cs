using BenchmarkDotNet.Running;
using System.IO;

namespace JsonParsingBenchmark
{
    class Program
    {
        static void Main()
        {
            var path = Startup.GetTestDataPath();
            Directory.CreateDirectory(path);
            Startup.EnsureTestData(path);

            BenchmarkRunner.Run<FullResultBenchmarks>();
        }
    }
}
