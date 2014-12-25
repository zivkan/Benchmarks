using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml;

namespace LocalisedResourceExtractionBenchmark.Extractions
{
    class LabelsAsXml : ISourceRepository
    {
        private readonly SqlConnection _connection;

        public LabelsAsXml(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<SourceModel> GetData()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT \n" +
                                      "s.Code, p.Code Parent, (SELECT Lang, Label FROM Dictionary l WHERE l.Id = s.Labels FOR XML AUTO, ROOT('d')) as Labels \n" +
                                      "FROM Source s \n" +
                                      "LEFT OUTER JOIN Source p on s.Parent=p.Id";
                using (var reader = command.ExecuteReader())
                {
                    var codeIndex = reader.GetOrdinal("Code");
                    var parentIndex = reader.GetOrdinal("Parent");
                    var labelsIndex = reader.GetOrdinal("Labels");
                    while (reader.Read())
                    {
                        var code = (string)reader[codeIndex];
                        var parentDbValue = reader[parentIndex];
                        var parent = parentDbValue == DBNull.Value ? null : (string) parentDbValue;
                        var labels = new Dictionary<string, string>();
                        using (var text = reader.GetTextReader(labelsIndex))
                        using (var xml = XmlReader.Create(text))
                        {
                            while (xml.Read())
                            {
                                if (xml.NodeType == XmlNodeType.Element && xml.Name == "l")
                                {
                                    var lang = xml.GetAttribute("Lang");
                                    var label = xml.GetAttribute("Label");
                                    labels.Add(lang, label);
                                }
                            }
                        }
                        yield return new SourceModel(code, parent, labels);
                    }
                }
            }
        }
    }
}
