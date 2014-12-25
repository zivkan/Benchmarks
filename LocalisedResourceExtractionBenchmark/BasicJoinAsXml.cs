using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml;

namespace LocalisedResourceExtractionBenchmark
{
    class BasicJoinAsXml : ISourceRepository
    {
        private readonly SqlConnection _connection;

        public BasicJoinAsXml(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<SourceModel> GetData()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = BasicJoin.CommandText + "\n FOR XML AUTO";
                using (var reader = command.ExecuteXmlReader())
                {
                    string code = null;
                    string parent = null;
                    Dictionary<string, string> labels = null;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "s")
                            {
                                code = reader.GetAttribute("Code");
                                parent = null;
                                labels = new Dictionary<string, string>();
                            }
                            else if (reader.Name == "p")
                            {
                                parent = reader.GetAttribute("Parent");
                            }
                            else if (reader.Name == "l")
                            {
                                labels.Add(reader.GetAttribute("Lang"), reader.GetAttribute("Label"));
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            if (reader.Name == "s")
                            {
                                yield return new SourceModel(code, parent, labels);
                            }
                        }
                    }
                }
            }
        }
    }
}
