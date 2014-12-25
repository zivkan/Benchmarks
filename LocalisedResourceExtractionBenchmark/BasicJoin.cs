using System;
using System.Collections.Generic;
using System.Data;

namespace LocalisedResourceExtractionBenchmark
{
    public class BasicJoin : ISourceRepository
    {
        private readonly IDbConnection _connection;

        public static string CommandText =
            "SELECT \n" + "s.Code, p.Code Parent, l.Lang, l.Label \n" + "FROM Source s \n" +
            "LEFT OUTER JOIN Source p on s.Parent=p.Id \n" + "LEFT OUTER JOIN Dictionary l on s.Labels=l.Id \n" +
            "ORDER BY s.Id";

        public BasicJoin(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<SourceModel> GetData()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = CommandText;
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        yield break;

                    var code = (string)reader["Code"];
                    var parentDbValue = reader["Parent"];
                    var parent = DBNull.Value == parentDbValue ? null : (string)parentDbValue;
                    var labels = new Dictionary<string, string>();
                    labels.Add((string) reader["Lang"], (string) reader["Label"]);

                    while (reader.Read())
                    {
                        var nextCode = (string)reader["Code"];
                        if (nextCode != code)
                        {
                            yield return new SourceModel(code, parent, labels);
                            code = nextCode;
                            parentDbValue = reader["Parent"];
                            parent = DBNull.Value == parentDbValue ? null : (string)parentDbValue;
                            labels = new Dictionary<string, string>();
                        }
                        labels.Add((string) reader["Lang"], (string) reader["Label"]);
                    }

                    yield return new SourceModel(code, parent, labels);
                }
            }
        }

    }
}