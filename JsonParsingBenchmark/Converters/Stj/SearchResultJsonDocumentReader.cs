using JsonParsingBenchmark.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonParsingBenchmark.Converters.Stj
{
    static class SearchResultJsonDocumentReader
    {
        public static SearchResults Parse(JsonElement rootElement)
        {
            var searchResults = new SearchResults();
            searchResults.TotalHits = rootElement.GetProperty("totalHits").GetInt32();
            var dataListElement = rootElement.GetProperty("data");
            var searchResultsData = new List<SearchResult>(dataListElement.GetArrayLength());
            foreach (var dataItemElement in dataListElement.EnumerateArray())
            {
                var searchResult = new SearchResult();
                searchResult.Id = dataItemElement.GetProperty("id").GetString();
                searchResult.Version = dataItemElement.GetProperty("version").GetString();
                searchResult.Description = dataItemElement.GetProperty("description").GetString();

                var searchResultVersionsElement = dataItemElement.GetProperty("versions");
                var searchResultVersions = new List<SearchResultVersion>(searchResultVersionsElement.GetArrayLength());
                foreach (var version in searchResultVersionsElement.EnumerateArray())
                {
                    var searchResultVersion = new SearchResultVersion();
                    searchResultVersion.Id = version.GetProperty("@id").GetString();
                    searchResultVersion.Version = version.GetProperty("version").GetString();
                    searchResultVersion.Downloads = version.GetProperty("downloads").GetInt64();

                    searchResultVersions.Add(searchResultVersion);
                }
                searchResult.Versions = searchResultVersions;

                searchResult.Authors = ParseStringArray(dataItemElement.GetProperty("authors"));

                if(dataItemElement.TryGetProperty("iconUrl", out JsonElement iconUrlProperty))
                {
                    searchResult.IconUrl = iconUrlProperty.GetString();
                }
                searchResult.LicenseUrl = dataItemElement.GetProperty("licenseUrl").GetString();
                searchResult.Owners = ParseStringArray(dataItemElement.GetProperty("owners"));
                searchResult.ProjectUrl = dataItemElement.GetProperty("projectUrl").GetString();
                searchResult.Registration = dataItemElement.GetProperty("registration").GetString();
                searchResult.Summary = dataItemElement.GetProperty("summary").GetString();
                searchResult.Tags = ParseStringArray(dataItemElement.GetProperty("tags"));
                searchResult.Title = dataItemElement.GetProperty("title").GetString();
                searchResult.TotalDownloads = dataItemElement.GetProperty("totalDownloads").GetInt64();
                if(dataItemElement.TryGetProperty("verified", out JsonElement verifiedProperty))
                {
                    searchResult.Verified = verifiedProperty.GetBoolean();
                }

                searchResultsData.Add(searchResult);
            }
            return searchResults;
        }


        private static List<string> ParseStringArray(JsonElement jsonElement)
        {
            var stringList = new List<string>(jsonElement.GetArrayLength());
            foreach (var author in jsonElement.EnumerateArray())
            {
                stringList.Add(author.GetString());
            }
            return stringList;
        }
    }
}
