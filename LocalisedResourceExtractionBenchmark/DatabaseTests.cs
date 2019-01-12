using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BenchmarkDotNet.Attributes;
using LocalisedResourceExtractionBenchmark.Extractions;
using LocalisedResourceExtractionBenchmark.Setup;

namespace LocalisedResourceExtractionBenchmark
{
    [MemoryDiagnoser]
    [SimpleJob(invocationCount: 10)] // automatic invocation count didn't run enough to trigger gen2 gc for some tests
    public class DatabaseTests
    {
        private const string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Database=LocalisedResourceExtractionBenchmark;Integrated Security=true";

        public void CreateTablesAndData()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var langs = new[] { "en", "fr", "de", "it", "es" };

                var db = new DbInitialise(connection, 20000, langs);
                db.CreateAndPopulateTables();
            }
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetExtractors))]
        public List<SourceModel> RunExtractions(Type extractor)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var constuctor = extractor.GetConstructor(new[] { typeof(SqlConnection) });
                var repo = (ISourceRepository)constuctor.Invoke(new[] { connection });

                return repo.GetData().ToList();
            }
        }

        public static IEnumerable<Type> GetExtractors()
        {
            yield return typeof(BasicJoin);
            yield return typeof(BasicJoinAsXml);
            yield return typeof(LabelsAsXml);
            yield return typeof(LabelsAsJoinedColumns);
            yield return typeof(LabelsAsPivotedColumns);
            yield return typeof(SingleLanguage);
            yield return typeof(LocalJoin);
        }
    }
}
