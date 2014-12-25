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

        [TestMethod, Ignore]
        public void CreateTablesAndData()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                Console.WriteLine(connection.ConnectionString);

                CreateTables(connection);

                PopulateTables(connection);
            }
        }

        private void CreateTables(SqlConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Dictionary (" +
                                      "Id INT NOT NULL, " +
                                      "Lang CHAR(2) NOT NULL, " +
                                      "Label NVARCHAR(MAX) NOT NULL,"+
                                      "PRIMARY KEY (Id,Lang))";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE Source (" +
                                      "Id INT IDENTITY NOT NULL, " +
                                      "Code VARCHAR(50) NOT NULL, " +
                                      "Parent INT," +
                                      "Labels INT NOT NULL,"+
                                      "PRIMARY KEY(Id))";
                command.ExecuteNonQuery();

            }
        }

        private void PopulateTables(SqlConnection connection)
        {
            const int maxRows = 500000;
            var dictionary = new TestDataBulkReader(TestDataBulkReader.Table.Dictionary, maxRows);
            var source = new TestDataBulkReader(TestDataBulkReader.Table.Source, maxRows);

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.BatchSize = 100000;
                bulkCopy.DestinationTableName = "Dictionary";
                bulkCopy.ColumnMappings.Add("Id", "Id");
                bulkCopy.ColumnMappings.Add("Lang", "Lang");
                bulkCopy.ColumnMappings.Add("Label", "Label");
                bulkCopy.WriteToServer(dictionary);

                bulkCopy.DestinationTableName = "Source";
                bulkCopy.ColumnMappings.Clear();
                bulkCopy.ColumnMappings.Add("Id", "Id");
                bulkCopy.ColumnMappings.Add("Code", "Code");
                bulkCopy.ColumnMappings.Add("Parent", "Parent");
                bulkCopy.ColumnMappings.Add("Labels", "Labels");
                bulkCopy.WriteToServer(source);
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

    }
}
