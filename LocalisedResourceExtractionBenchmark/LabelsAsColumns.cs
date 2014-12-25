using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LocalisedResourceExtractionBenchmark
{
    class LabelsAsColumns : ISourceRepository
    {
        private readonly IDbConnection _connection;

        public LabelsAsColumns(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<SourceModel> GetData()
        {
            using (var command = _connection.CreateCommand())
            {
                var langs = GetLanguages(command);

                command.CommandText = GenerateCommandText(langs);

                using (var reader = command.ExecuteReader())
                {
                    const int codeIndex = 0;
                    const int parentIndex = 1;
                    const int labelsIndex = 2;
                    while (reader.Read())
                    {
                        var code = (string) reader[codeIndex];
                        var parentValue = reader[parentIndex];
                        var parent = parentValue == DBNull.Value ? null : (string) parentValue;
                        var labels = new Dictionary<string, string>();
                        for (int i = 0; i < langs.Count; i++)
                        {
                            var labelValue = reader[labelsIndex + i];
                            var label = labelValue == DBNull.Value ? null : (string) labelValue;
                            labels.Add(langs[i], label);
                        }
                        yield return new SourceModel(code, parent, labels);
                    }
                }
            }
        }

        private IList<string> GetLanguages(IDbCommand command)
        {
            command.CommandText = "SELECT Lang FROM Languages";
            using (var reader = command.ExecuteReader())
            {
                var langs = new List<string>();
                while (reader.Read())
                    langs.Add((string)reader[0]);
                return langs;
            }
        }

        private string GenerateCommandText(IList<string> langs)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT s.Code, p.Code Parent");
            foreach (var lang in langs)
                sb.AppendFormat(", l_{0}.Label Label{0}", lang);
            sb.Append("\nFROM Source s\nLEFT OUTER JOIN Source p ON s.Parent=p.Id");
            foreach (var lang in langs)
                sb.AppendFormat("\nLEFT OUTER JOIN Dictionary l_{0} ON s.Labels=l_{0}.Id AND l_{0}.Lang='{0}'", lang);
            return sb.ToString();
        }
        
    }
}
