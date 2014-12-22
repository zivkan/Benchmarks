using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocalisedResourceExtractionBenchmark
{
    [TestClass]
    public class DatabaseTests
    {
        private const string ConnectionString = "Data Source=(LocalDB)\\ProjectsV12;Integrated Security=true";

        [TestMethod]
        public void CreateTablesAndData()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                Console.WriteLine(connection.ConnectionString);

                CreateTables(connection);
            }
        }

        private void CreateTables(SqlConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Dictionary (" +
                                      "Id INT, " +
                                      "Lang CHAR(2), " +
                                      "Label NVARCHAR(MAX) NOT NULL,"+
                                      "PRIMARY KEY (Id,Lang))";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE Source (" +
                                      "Id INT PRIMARY KEY IDENTITY, " +
                                      "Code VARCHAR(50) NOT NULL, " +
                                      "Parent INT," +
                                      "Labels INT NOT NULL)";
                command.ExecuteNonQuery();

            }
        }
    }
}
