using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace JsonParsingBenchmark
{
    public static class Startup
    {
        public static string GetTestDataPath()
        {
            var path = Path.GetDirectoryName(typeof(Startup).Assembly.Location);
            var checkFile = Path.Combine(path, ".gitignore");
            while (!File.Exists(checkFile))
            {
                var newPath = Path.GetDirectoryName(path);
                if (newPath == path)
                {
                    throw new FileNotFoundException();
                }

                path = newPath;
                checkFile = Path.Combine(path, ".gitignore");
            }

            return Path.Combine(path, "TestData", nameof(JsonParsingBenchmark));
        }

        public static void EnsureTestData(string path)
        {
            EnsureTestData(Path.Combine(path, "nuget-org.json"), "https://api.nuget.org/v3/index.json");
            EnsureTestData(Path.Combine(path, "dotnet-core.json"), "https://dotnetfeed.blob.core.windows.net/dotnet-core/index.json");
        }

        private static void EnsureTestData(string fileName, string url)
        {
            if (!File.Exists(fileName))
            {
                using (var httpHandler = new HttpClientHandler()
                {
                    AutomaticDecompression =
#if NETFRAMEWORK
                    DecompressionMethods.GZip | DecompressionMethods.Deflate
#else
                    DecompressionMethods.All
#endif
                })
                using (var client = new HttpClient(httpHandler))
                {
                    var searchUrl = GetSearchUrl(client, url) + "?skip=0&take=26";

                    using (var request = new HttpRequestMessage(HttpMethod.Get, searchUrl))
                    {
                        var requestTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                        requestTask.Wait();
                        var response = requestTask.Result;
                        var contentStreamTask = response.Content.ReadAsStreamAsync();
                        contentStreamTask.Wait();
                        using (var httpStream = contentStreamTask.Result)
                        using (var fileStream = File.OpenWrite(fileName))
                        {
                            httpStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }

        private static string GetSearchUrl(HttpClient client, string url)
        {
            var responseTask = client.GetStringAsync(url);
            responseTask.Wait();
            var indexJson = responseTask.Result;

            var document = JsonDocument.Parse(indexJson);

            var resources = document.RootElement.GetProperty("resources");
            foreach (var resource in resources.EnumerateArray())
            {
                string id = null;
                string type = null;

                foreach (var property in resource.EnumerateObject())
                {
                    if (property.Name == "@id")
                    {
                        id = property.Value.GetString();
                    }
                    else if (property.Name == "@type")
                    {
                        type = property.Value.GetString();
                    }
                }

                if (type?.StartsWith("SearchQueryService/") ?? false)
                {
                    return id ?? throw new Exception("search query service url null");
                }
            }

            throw new Exception("search query service not found");
        }

        public static IReadOnlyDictionary<string, string> LoadTestData()
        {
            var data = new Dictionary<string, string>();

            var directory = GetTestDataPath();
            var files = Directory.EnumerateFiles(directory);

            foreach (var fullPath in files)
            {
                var fileName = Path.GetFileName(fullPath);
                data.Add(fileName, fullPath);
            }

            return data;
        }
    }
}
