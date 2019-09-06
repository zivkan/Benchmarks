using JsonParsingBenchmark.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JsonParsingBenchmark.Converters.Nj
{
    internal class SearchResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SearchResult);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(SearchResult))
            {
                throw new InvalidOperationException();
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonException("Expected StartObjected, found " + reader.TokenType);
            }

            var searchResult = new SearchResult();

        bool finished = false;
            while (!finished)
            {
                reader.Read();
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        switch ((string)reader.Value)
                        {
                            case "id":
                                searchResult.Id = reader.ReadAsString();
                                break;

                            case "version":
                                searchResult.Version = reader.ReadAsString();
                                break;

                            case "description":
                                searchResult.Description = reader.ReadAsString();
                                break;

                            case "versions":
                                searchResult.Versions = new List<SearchResultVersion>();
                                reader.Read();
                                if (reader.TokenType != JsonToken.StartArray)
                                {
                                    throw new JsonException("Expected StartArray, found " + reader.TokenType);
                                }
                                reader.Read();
                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var version = serializer.Deserialize<SearchResultVersion>(reader);
                                    searchResult.Versions.Add(version);
                                    reader.Read();
                                }
                                break;

                            case "authors":
                                searchResult.Authors = new List<string>();
                                reader.Read();
                                if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array");
                                reader.Read();
                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    if (reader.TokenType == JsonToken.String)
                                    {
                                        var author = (string)reader.Value;
                                        searchResult.Authors.Add(author);
                                    }
                                    else
                                    {
                                        throw new JsonException("Expected string");
                                    }
                                    reader.Read();
                                }
                                break;

                            case "iconUrl":
                                searchResult.IconUrl = reader.ReadAsString();
                                break;

                            case "licenseUrl":
                                searchResult.LicenseUrl = reader.ReadAsString();
                                break;

                            case "owners":
                                searchResult.Owners = new List<string>();
                                reader.Read();
                                if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array");
                                reader.Read();
                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    if (reader.TokenType == JsonToken.String)
                                    {
                                        var owner = (string)reader.Value;
                                        searchResult.Owners.Add(owner);
                                    }
                                    else
                                    {
                                        throw new JsonException("Expected string");
                                    }
                                    reader.Read();
                                }
                                break;

                            case "projectUrl":
                                searchResult.ProjectUrl = reader.ReadAsString();
                                break;

                            case "registration":
                                searchResult.Registration = reader.ReadAsString();
                                break;

                            case "summary":
                                searchResult.Summary = reader.ReadAsString();
                                break;

                            case "tags":
                                searchResult.Tags = new List<string>();
                                reader.Read();
                                if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array");
                                reader.Read();
                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    if (reader.TokenType == JsonToken.String)
                                    {
                                        var tag = (string)reader.Value;
                                        searchResult.Tags.Add(tag);
                                    }
                                    else
                                    {
                                        throw new JsonException("Expected string");
                                    }
                                    reader.Read();
                                }
                                break;

                            case "title":
                                searchResult.Title = reader.ReadAsString();
                                break;

                            case "totalDownloads":
                                searchResult.TotalDownloads = reader.ReadAsInt32() ?? 0;
                                break;

                            case "verified":
                                searchResult.Verified = reader.ReadAsBoolean() ?? false;
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
                        throw new JsonException("Unexpected type " + reader.TokenType);
                }
            }

            return searchResult;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
