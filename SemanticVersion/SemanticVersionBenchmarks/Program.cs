using BenchmarkDotNet.Running;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticVersionBenchmarks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

            //bool showHelp = true;
            //if (args.Length > 0)
            //{
            //    var which = args[0];
            //    Type? benchmark = which switch
            //    {
            //        "sort" => typeof(SortBenchmarks),
            //        "parse" => typeof(ParseBenchmarks),
            //        _ => null
            //    };

            //    if (benchmark != null)
            //    {
            //        showHelp = false;

            //        var versions = await VersionData.GetVersionsAsync();
            //        BenchmarkRunner.Run(benchmark);
            //    }
            //}

            //if (showHelp)
            //{
            //    Console.WriteLine("Usage: {0} <benchmark>", typeof(Program).Assembly.GetName().Name);
            //    Console.WriteLine("<benchmark> is one of: sort, parse");
            //}
        }
    }
}
