using JsonParsingBenchmark.Model;
using Newtonsoft.Json;
using System;

namespace JsonParsingBenchmark.Converters.Nj
{
    internal class SearchResultVersionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SearchResultVersion);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(SearchResultVersion))
            {
                throw new InvalidOperationException();
            }

            if (reader.TokenType != JsonToken.StartObject)
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
                    case JsonToken.PropertyName:
                        switch ((string)reader.Value)
                        {
                            case "@id":
                                version.Id = reader.ReadAsString();
                                break;

                            case "version":
                                version.Version = reader.ReadAsString();
                                break;

                            case "downloads":
                                version.Downloads = reader.ReadAsInt32() ?? 0;
                                break;

                            default:
                                reader.Skip();
                                break;
                        }
                        break;

                    case JsonToken.EndObject:
                        finished = true;
                        break;

                    default:
                        throw new JsonException("Unexpected " + reader.TokenType);
                }
            }

            return version;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
