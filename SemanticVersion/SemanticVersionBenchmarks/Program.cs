using BenchmarkDotNet.Running;
using System.Threading.Tasks;

namespace SemanticVersionBenchmarks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var versions = await VersionData.GetVersionsAsync();

            //BenchmarkRunner.Run<SortBenchmarks>();
            BenchmarkRunner.Run<ParseBenchmarks>();
        }
    }
}
