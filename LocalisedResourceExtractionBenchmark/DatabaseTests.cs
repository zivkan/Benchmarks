using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocalisedResourceExtractionBenchmark
{
    [TestClass]
    public class DatabaseTests
    {
        private const string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Database=LocalisedResourceExtractionBenchmark;Integrated Security=true";

        [TestMethod]
        public void CreateTablesAndData()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var langs = new[] {"en", "fr", "de", "it", "es"};

                var db = new DbInitialise(connection, 500000, langs);
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

        [TestMethod]
        public void BasicJoinTests()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                ISourceRepository repo = new BasicJoin(connection);
                RunTest(repo);
            }
        }

        [TestMethod]
        public void BasicJoinAsXmlTests()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                ISourceRepository repo = new BasicJoinAsXml(connection);
                RunTest(repo);
            }
        }

        [TestMethod]
        public void LabelsAsXmlTests()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                ISourceRepository repo = new LabelsAsXml(connection);
                RunTest(repo);
            }
        }

        [TestMethod]
        public void LabelsAsColumnsTests()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                ISourceRepository repo = new LabelsAsColumns(connection);
                RunTest(repo);
            }
        }

    }
}
