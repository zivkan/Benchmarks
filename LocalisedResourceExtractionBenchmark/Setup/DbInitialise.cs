using System.Collections.Generic;
using System.Data.SqlClient;

namespace LocalisedResourceExtractionBenchmark.Setup
{
    class DbInitialise
    {
        private readonly SqlConnection _connection;
        private readonly int _numSourcers;
        private readonly IList<string> _languages;

        public DbInitialise(SqlConnection connection, int numSourcers, IList<string> languages)
        {
            _connection = connection;
            _numSourcers = numSourcers;
            _languages = languages;
        }

        public void CreateAndPopulateTables()
        {
            CreateLanguageTable();
            CreateDictionaryTable();
            CreateSourceTable();

            PopulateLanguageTable();
            PopulateDictionaryTable();
            PopulateSourceTable();
        }

        private void CreateLanguageTable()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Languages (Lang CHAR(2) NOT NULL PRIMARY KEY)";
                command.ExecuteNonQuery();
            }
        }

        private void CreateDictionaryTable()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Dictionary (" +
                                      "Id INT NOT NULL, " +
                                      "Lang CHAR(2) NOT NULL, " +
                                      "Label NVARCHAR(MAX) NOT NULL," +
                                      "PRIMARY KEY (Id,Lang)," +
                                      "FOREIGN KEY (Lang) REFERENCES Languages(Lang))";
                command.ExecuteNonQuery();
            }
        }

        private void CreateSourceTable()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Source (" +
                                      "Id INT IDENTITY NOT NULL, " +
                                      "Code VARCHAR(50) NOT NULL, " +
                                      "Parent INT," +
                                      "Labels INT NOT NULL," +
                                      "PRIMARY KEY(Id))";
                command.ExecuteNonQuery();
            }
        }

        private void PopulateLanguageTable()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Languages (Lang) VALUES (@lang)";
                var param = command.Parameters.Add(command.CreateParameter());
                param.ParameterName = "@lang";
                foreach (var lang in _languages)
                {
                    param.Value = lang;
                    command.ExecuteNonQuery();
                }
            }
        }

        private void PopulateDictionaryTable()
        {
            using (var bulk = new SqlBulkCopy(_connection))
            {
                var dictionary = new TestDataBulkReader(TestDataBulkReader.Table.Dictionary, _numSourcers);

                bulk.BatchSize = 100000;
                bulk.DestinationTableName = "Dictionary";
                bulk.ColumnMappings.Add("Id", "Id");
                bulk.ColumnMappings.Add("Lang", "Lang");
                bulk.ColumnMappings.Add("Label", "Label");
                bulk.WriteToServer(dictionary);
            }
        }

        private void PopulateSourceTable()
        {
            using (var bulk = new SqlBulkCopy(_connection))
            {
                var source = new TestDataBulkReader(TestDataBulkReader.Table.Source, _numSourcers);

                bulk.BatchSize = 100000;
                bulk.DestinationTableName = "Source";
                bulk.ColumnMappings.Add("Id", "Id");
                bulk.ColumnMappings.Add("Code", "Code");
                bulk.ColumnMappings.Add("Parent", "Parent");
                bulk.ColumnMappings.Add("Labels", "Labels");
                bulk.WriteToServer(source);
            }
        }
    }
}
