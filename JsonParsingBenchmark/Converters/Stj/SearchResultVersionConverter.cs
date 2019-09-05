using JsonParsingBenchmark.Model;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonParsingBenchmark.Converters.Stj
{
    class SearchResultVersionConverter : JsonConverter<SearchResultVersion>
    {
        private static readonly byte[] utf8Id = Encoding.UTF8.GetBytes("@id");
        private static readonly byte[] utf8Version = Encoding.UTF8.GetBytes("version");
        private static readonly byte[] utf8Downloads = Encoding.UTF8.GetBytes("downloads");

        public override SearchResultVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert != typeof(SearchResultVersion))
            {
                throw new InvalidOperationException();
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject, found " + reader.TokenType);
            }

            var version = new SearchResultVersion();

            bool finished = false;
            while (!finished)
            {
                reader.Read();
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        if (reader.ValueTextEquals(utf8Id))
                        {
                            reader.Read();
                            version.Id = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8Version))
                        {
                            reader.Read();
                            version.Version = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8Downloads))
                        {
                            reader.Read();
                            version.Downloads = reader.GetInt32();
                        }
                        else
                        {
                            reader.Skip();
                        }
                        break;

                    case JsonTokenType.EndObject:
                        finished = true;
                        break;

                    default:
                        throw new JsonException("Unexpected " + reader.TokenType);
                }
            }

            return version;
        }

        public override void Write(Utf8JsonWriter writer, SearchResultVersion value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
