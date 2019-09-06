using JsonParsingBenchmark.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JsonParsingBenchmark.Converters.Nj
{
    internal class SearchResultsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SearchResults);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType!= typeof(SearchResults))
            {
                throw new InvalidOperationException();
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonException("Expected StartObject, found " + reader.TokenType);
            }

            var searchResults = new SearchResults();

            var finished = false;
            while (!finished)
            {
                reader.Read();

                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        switch ((string)reader.Value)
                        {
                            case "totalHits":
                                searchResults.TotalHits = reader.ReadAsInt32() ?? 0;
                                break;

                            case "data":
                                searchResults.Data = new List<SearchResult>();

                                reader.Read();
                                if (reader.TokenType != JsonToken.StartArray) throw new JsonException("data should be an array");
                                reader.Read();
                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var searchResult = serializer.Deserialize<SearchResult>(reader);
                                    searchResults.Data.Add(searchResult);
                                    reader.Read();
                                }

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
                        throw new JsonException();
                }
            }

            return searchResults;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
