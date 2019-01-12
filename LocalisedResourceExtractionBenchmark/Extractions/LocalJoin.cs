using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LocalisedResourceExtractionBenchmark.Extractions
{
    public class LocalJoin : ISourceRepository
    {
        private readonly IDbConnection _connection;

        public LocalJoin(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<SourceModel> GetData()
        {
            using (var command = _connection.CreateCommand())
            {
                var sourceData = GetSourceData(command);
                var labels = GetLabels(command, sourceData);

                foreach (var source in sourceData)
                {
                    var parentCode = source.Value.ParentId.HasValue ? sourceData[source.Value.ParentId.Value].Code : null;
                    var l = labels[source.Value.LabelId];

                    var s = new SourceModel(source.Value.Code, parentCode, l);

                    yield return s;
                }
            }
        }

        private Dictionary<int, SourceRow> GetSourceData(IDbCommand command)
        {
            var result = new Dictionary<int, SourceRow>();

            command.CommandText = "SELECT Id, Code, Parent, Labels FROM Source";

            using (var reader = command.ExecuteReader())
            {
                const int idIndex = 0;
                const int codeIndex = 1;
                const int parentIndex = 2;
                const int labelIndex = 3;
                while (reader.Read())
                {
                    var row = new SourceRow();
                    int id = (int)reader[idIndex];
                    row.Code = (string)reader[codeIndex];
                    var parentValue = reader[parentIndex];
                    row.ParentId = parentValue == DBNull.Value ? (int?)null : (int)parentValue;
                    row.LabelId = (int)reader[labelIndex];

                    result.Add(id, row);
                }
            }

            return result;
        }

        private Dictionary<int, Dictionary<string, string>> GetLabels(IDbCommand command, Dictionary<int, SourceRow> sourceData)
        {
            var result = new Dictionary<int, Dictionary<string, string>>();

            command.CommandText = GenerateGetLabelsQuery(command, sourceData);

            using (var reader = command.ExecuteReader())
            {
                const int idIndex = 0;
                const int langIndex = 1;
                const int labelIndex = 2;

                int lastId = int.MinValue;
                Dictionary<string, string> labels = null;

                while (reader.Read())
                {
                    int id = (int)reader[idIndex];
                    string lang = (string)reader[langIndex];
                    string label = (string)reader[labelIndex];

                    if (lastId != id)
                    {
                        if (!result.TryGetValue(id, out labels))
                        {
                            labels = new Dictionary<string, string>();
                            result.Add(id, labels);
                        }
                    }

                    labels.Add(lang, label);
                }
            }

            return result;
        }

        private string GenerateGetLabelsQuery(IDbCommand command, Dictionary<int, SourceRow> sourceData)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT Id, Lang, Label FROM Dictionary WHERE Id IN (");
            foreach (var s in sourceData)
            {
                sb.AppendFormat("{0}, ", s.Value.LabelId);
            }
            sb.Length -= 2;
            sb.Append(")");

            return sb.ToString();
        }

        private class SourceRow
        {
            public string Code { get; set; }
            public int? ParentId { get; set; }
            public int LabelId { get; set; }
        }
    }
}
