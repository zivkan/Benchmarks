using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LocalisedResourceExtractionBenchmark.Benchmark;
using LocalisedResourceExtractionBenchmark.Extractions;
using LocalisedResourceExtractionBenchmark.Setup;
using NUnit.Framework;

namespace LocalisedResourceExtractionBenchmark
{
    [TestFixture]
    public class DatabaseTests
    {
        private const string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Database=LocalisedResourceExtractionBenchmark;Integrated Security=true";

        [Test]
        public void CreateTablesAndData()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var langs = new[] {"en", "fr", "de", "it", "es"};

                var db = new DbInitialise(connection, 20000, langs);
                db.CreateAndPopulateTables();
            }
        }

        private static void RunTest(ISourceRepository repo)
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

            Console.WriteLine("{0} rows. First row after {1}s, complete after {2}s", rows,
                latencyTimer.Elapsed.TotalSeconds, completeTimer.Elapsed.TotalSeconds);
        }

        [Test]

        public void RunExtractions(
            [Values(typeof (BasicJoin),
                typeof (BasicJoinAsXml),
                typeof (LabelsAsXml),
                typeof (LabelsAsColumns),
                typeof(SingleLanguage))] Type type)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var constuctor = type.GetConstructor(new[] {typeof (SqlConnection)});
                var repo = (ISourceRepository)constuctor.Invoke(new[] {connection});

                RunTest(repo);
            }
        }


        [Test]
        public void RunBenchmark()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                ISourceRepository[] extractors =
                {
                    new BasicJoin(connection),
                    new BasicJoinAsXml(connection),
                    new LabelsAsColumns(connection),
                    new LabelsAsXml(connection),
                };

                const int numRuns = 10;
                var benchmark = new BenchmarkRunner(extractors, new SingleLanguage(connection), numRuns);
                var results = benchmark.Run();

                var fastestResult = results.ExtractionResults[0];
                PrintResult(results.SingleLanguageResult, fastestResult);
                foreach (var result in results.ExtractionResults)
                {
                    PrintResult(result, fastestResult);
                }

                UpdateReadme(results);
            }
        }

        private static void PrintResult(Result result, Result fastestResult)
        {
            var firstPct = result.TimeToFirst*100.0/fastestResult.TimeToFirst;
            var completePct = result.TimeToComplete*100.0/fastestResult.TimeToComplete;
            Console.WriteLine("{0}: First {1:0.##}s ({2:#}%), Complete {3:0.##}s ({4:#}%)", result.Name,
                result.TimeToFirst, firstPct, result.TimeToComplete, completePct);
        }

        private void UpdateReadme(Results results)
        {
            var filename = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Readme.md");

            var contents = File.ReadAllLines(filename).ToList();
            var resultsIndex = contents.IndexOf("## <a id=\"results\">Results</a>");
            if (resultsIndex < 0)
                throw new Exception("Could not find results header");
            var countToRemove = contents.Count - resultsIndex - 1;
            contents.RemoveRange(resultsIndex+1, countToRemove);

            var fastestResult = results.ExtractionResults[0];

            contents.Add("");
            contents.Add("|Extraction Method|Time to first result|Time to complete|");
            contents.Add("|-----------------|--------------------|----------------|");
            contents.Add(ResultString(results.SingleLanguageResult, fastestResult));
            contents.AddRange(results.ExtractionResults.Select(result => ResultString(result, fastestResult)));

            File.WriteAllLines(filename, contents);
        }

        private string ResultString(Result result, Result fastestResult)
        {
            var firstPct = result.TimeToFirst*100.0/fastestResult.TimeToFirst;
            var completePct = result.TimeToComplete*100.0/fastestResult.TimeToComplete;
            return string.Format("|{0}|{1:0.##}s ({2:0}%)|{3:0.##}s ({4:0}%)|", result.Name, result.TimeToFirst, firstPct,
                result.TimeToComplete, completePct);
        }

    }
}
