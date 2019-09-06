using BenchmarkDotNet.Attributes;
using JsonParsingBenchmark.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonParsingBenchmark
{
    [MemoryDiagnoser]
    public class FullResultBenchmarks
    {
        private byte[] data;
        private TestServer testServer;
        private HttpClient httpClient;
        private Uri uri = new Uri("/", UriKind.Relative);

        [GlobalSetup]
        public void Setup()
        {
            data = Startup.LoadTestData();

            var whb = new WebHostBuilder()
                .Configure(app =>
                {
                    app.Run(async handler =>
                    {
                        handler.Response.ContentType = "application/json";
                        handler.Response.ContentLength = data.Length;
                        await handler.Response.StartAsync().ConfigureAwait(false);

                        using (var stream = handler.Response.BodyWriter.AsStream())
                        {
                            await stream.WriteAsync(data).ConfigureAwait(false);
                        }
                    });
                });

            testServer = new TestServer(whb);
            httpClient = testServer.CreateClient();
        }


        [GlobalCleanup]
        public void Cleanup()
        {
            httpClient?.Dispose();
            testServer?.Dispose();
        }

        [Benchmark(Description = "System.Text.Json", Baseline = true)]
        public async Task<SearchResults> SystemTextJson()
        {
            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var obj = await JsonSerializer.DeserializeAsync<SearchResults>(stream);

            return obj;
        }

        [Benchmark(Description = "System.Text.Json with JsonConverters")]
        public async Task<SearchResults> SystemTextJsonCustomConverter()
        {
            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new Converters.Stj.SearchResultsConverter());
            options.Converters.Add(new Converters.Stj.SearchResultConverter());
            options.Converters.Add(new Converters.Stj.SearchResultVersionConverter());

            var obj = await JsonSerializer.DeserializeAsync<SearchResults>(stream, options: options).ConfigureAwait(false);

            return obj;
        }

        [Benchmark(Description = "Newtonsoft.Json")]
        public async Task<SearchResults> NewtonsoftJson()
        {
            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            SearchResults obj;
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                obj = serializer.Deserialize<SearchResults>(jsonReader);
            }

            return obj;
        }

        [Benchmark(Description = "Newtonsoft.Json with JsonConverters")]
        public async Task<SearchResults> NewtonsoftJsonCustomConverter()
        {
            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            SearchResults obj;
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Converters.Add(new Converters.Nj.SearchResultsConverter());
                serializer.Converters.Add(new Converters.Nj.SearchResultConverter());
                serializer.Converters.Add(new Converters.Nj.SearchResultVersionConverter());

                obj = serializer.Deserialize<SearchResults>(jsonReader);
            }

            return obj;
        }
    }
}
