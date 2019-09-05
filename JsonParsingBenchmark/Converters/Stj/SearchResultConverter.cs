using JsonParsingBenchmark.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonParsingBenchmark.Converters.Stj
{
    class SearchResultConverter : JsonConverter<SearchResult>
    {
        private static readonly byte[] utf8Id = Encoding.UTF8.GetBytes("id");
        private static readonly byte[] utf8Version = Encoding.UTF8.GetBytes("version");
        private static readonly byte[] utf8Description = Encoding.UTF8.GetBytes("description");
        private static readonly byte[] utf8Versions = Encoding.UTF8.GetBytes("versions");
        private static readonly byte[] utf8Authors = Encoding.UTF8.GetBytes("authors");
        private static readonly byte[] utf8IconUrl = Encoding.UTF8.GetBytes("iconUrl");
        private static readonly byte[] utf8LicenseUrl = Encoding.UTF8.GetBytes("licenseUrl");
        private static readonly byte[] utf8Owners = Encoding.UTF8.GetBytes("owners");
        private static readonly byte[] utf8ProjectUrl = Encoding.UTF8.GetBytes("projectUrl");
        private static readonly byte[] utf8Registration = Encoding.UTF8.GetBytes("registration");
        private static readonly byte[] utf8Summary = Encoding.UTF8.GetBytes("summary");
        private static readonly byte[] utf8Tags = Encoding.UTF8.GetBytes("tags");
        private static readonly byte[] utf8Title = Encoding.UTF8.GetBytes("title");
        private static readonly byte[] utf8TotalDownloads = Encoding.UTF8.GetBytes("totalDownloads");
        private static readonly byte[] utf8Verified = Encoding.UTF8.GetBytes("verified");

        public override SearchResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert != typeof(SearchResult))
            {
                throw new InvalidOperationException();
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObjected, found " + reader.TokenType);
            }

            var searchResult = new SearchResult();
            var versionConverter = (JsonConverter<SearchResultVersion>)options.GetConverter(typeof(SearchResultVersion));

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
                            searchResult.Id = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8Version))
                        {
                            reader.Read();
                            searchResult.Version = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8Description))
                        {
                            reader.Read();
                            searchResult.Description = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8Versions))
                        {
                            searchResult.Versions = new List<SearchResultVersion>();
                            reader.Read();
                            if (reader.TokenType != JsonTokenType.StartArray)
                            {
                                throw new JsonException("Expected StartArray, found " + reader.TokenType);
                            }
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                var version = versionConverter.Read(ref reader, typeof(SearchResultVersion), options);
                                searchResult.Versions.Add(version);
                                reader.Read();
                            }
                        }
                        else if (reader.ValueTextEquals(utf8Authors))
                        {
                            searchResult.Authors = new List<string>();
                            reader.Read();
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected start array");
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.String)
                                {
                                    var author = reader.GetString();
                                    searchResult.Authors.Add(author);
                                }
                                else
                                {
                                    throw new JsonException("Expected string");
                                }
                                reader.Read();
                            }
                        }
                        else if (reader.ValueTextEquals(utf8IconUrl))
                        {
                            reader.Read();
                            searchResult.IconUrl = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8LicenseUrl))
                        {
                            reader.Read();
                            searchResult.LicenseUrl = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8Owners))
                        {
                            searchResult.Owners = new List<string>();
                            reader.Read();
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected start array");
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.String)
                                {
                                    var owner = reader.GetString();
                                    searchResult.Owners.Add(owner);
                                }
                                else
                                {
                                    throw new JsonException("Expected string");
                                }
                                reader.Read();
                            }
                        }
                        else if (reader.ValueTextEquals(utf8ProjectUrl))
                        {
                            reader.Read();
                            searchResult.ProjectUrl = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8Registration))
                        {
                            reader.Read();
                            searchResult.Registration = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8Summary))
                        {
                            reader.Read();
                            searchResult.Summary = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8Tags))
                        {
                            searchResult.Tags = new List<string>();
                            reader.Read();
                            if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected start array");
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType == JsonTokenType.String)
                                {
                                    var tag = reader.GetString();
                                    searchResult.Tags.Add(tag);
                                }
                                else
                                {
                                    throw new JsonException("Expected string");
                                }
                                reader.Read();
                            }
                        }
                        else if (reader.ValueTextEquals(utf8Title))
                        {
                            reader.Read();
                            searchResult.Title = reader.GetString();
                        }
                        else if (reader.ValueTextEquals(utf8TotalDownloads))
                        {
                            reader.Read();
                            searchResult.TotalDownloads = reader.GetInt32();
                        }
                        else if (reader.ValueTextEquals(utf8Verified))
                        {
                            reader.Read();
                            searchResult.Verified = reader.GetBoolean();
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
                        throw new JsonException("Unexpected type " + reader.TokenType);
                }
            }

            return searchResult;
        }

        public override void Write(Utf8JsonWriter writer, SearchResult value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
