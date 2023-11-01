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
    public class DatabaseTests
    {
        private const string LocalDbConnectionString = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true";
        private const string DatabaseName = "LocalisedResourceExtractionBenchmark";
        private const string BenchmarkConnectionString = LocalDbConnectionString + ";Database=" + DatabaseName;

        public static void CreateTablesAndData()
        {
            using (var connection = new SqlConnection(LocalDbConnectionString))
            {
                connection.Open();
                bool dbExists;
                using (var command = new SqlCommand($"SELECT name FROM sys.databases WHERE name='{DatabaseName}'", connection))
                {
                    var result = command.ExecuteScalar();
                    dbExists = result != null;
                }

                if (!dbExists)
                {
                    using (var command = new SqlCommand($"CREATE DATABASE {DatabaseName}", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            using (var connection = new SqlConnection(BenchmarkConnectionString))
            {
                connection.Open();

                var langs = new[] { "en", "fr", "de", "it", "es" };

                var db = new DbInitialise(connection, 20000, langs);
                db.CreateAndPopulateTables();
            }
        }

        private List<SourceModel> RunExtractions(Type extractor)
        {
            using (var connection = new SqlConnection(BenchmarkConnectionString))
            {
                connection.Open();

                var constuctor = extractor.GetConstructor(new[] { typeof(SqlConnection) });
                var repo = (ISourceRepository)constuctor.Invoke(new[] { connection });

                return repo.GetData().ToList();
            }
        }

        [Benchmark]
        public List<SourceModel> BasicJoin()
        {
            return RunExtractions(typeof(BasicJoin));
        }

        [Benchmark]
        public List<SourceModel> BasicJoinAsXml()
        {
            return RunExtractions(typeof(BasicJoinAsXml));
        }

        [Benchmark]
        public List<SourceModel> LabelsAsXml()
        {
            return RunExtractions(typeof(LabelsAsXml));
        }

        [Benchmark]
        public List<SourceModel> LabelsAsJoinedColumns()
        {
            return RunExtractions(typeof(LabelsAsJoinedColumns));
        }

        [Benchmark]
        public List<SourceModel> LabelsAsPivotedColumns()
        {
            return RunExtractions(typeof(LabelsAsPivotedColumns));
        }

        [Benchmark]
        public List<SourceModel> SingleLanguage()
        {
            return RunExtractions(typeof(SingleLanguage));
        }

        [Benchmark]
        public List<SourceModel> LocalJoin()
        {
            return RunExtractions(typeof(LocalJoin));
        }
    }
}
