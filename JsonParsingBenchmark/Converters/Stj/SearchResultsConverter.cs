using JsonParsingBenchmark.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonParsingBenchmark.Converters.Stj
{
    class SearchResultsConverter : JsonConverter<SearchResults>
    {
        private static readonly byte[] utf8TotalHits = Encoding.UTF8.GetBytes("totalHits");
        private static readonly byte[] utf8Data = Encoding.UTF8.GetBytes("data");

        public override SearchResults Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert != typeof(SearchResults))
            {
                throw new InvalidOperationException();
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject, found " + reader.TokenType);
            }

            var searchResults = new SearchResults();
            var searchResultConverter = (JsonConverter<SearchResult>)options.GetConverter(typeof(SearchResult));

            var finished = false;
            while (!finished)
            {
                reader.Read();

                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        if (reader.ValueTextEquals(utf8TotalHits))
                        {
                            reader.Read();
                            searchResults.TotalHits = reader.GetInt32();
                        }
                        else if (reader.ValueTextEquals(utf8Data))
                        {
                            searchResults.Data = new List<SearchResult>();

                            reader.Read();
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("data should be an array");
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                var searchResult = searchResultConverter.Read(ref reader, typeof(SearchResult), options);
                                searchResults.Data.Add(searchResult);
                                reader.Read();
                            }
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
                        throw new JsonException();
                }
            }

            return searchResults;
        }

        public override void Write(Utf8JsonWriter writer, SearchResults value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
