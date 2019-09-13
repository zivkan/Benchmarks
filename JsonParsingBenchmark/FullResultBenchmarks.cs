using BenchmarkDotNet.Attributes;
using JsonParsingBenchmark.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Nj = Newtonsoft.Json;
using Stj = System.Text.Json;

namespace JsonParsingBenchmark
{
    [MemoryDiagnoser]
    public class FullResultBenchmarks
    {
        private TestServer testServer;
        private HttpClient httpClient;

        [GlobalSetup]
        public void Setup()
        {
            var data = Startup.LoadTestData();

            var whb = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    var route = new RouteBuilder(app);

                    foreach (var kvp in data)
                    {
                        route.MapGet("/" + kvp.Key, async handler =>
                    {
                        handler.Response.ContentType = "application/json";
                        handler.Response.ContentLength = kvp.Value.Length;
                        await handler.Response.StartAsync().ConfigureAwait(false);

                        using (var stream = handler.Response.BodyWriter.AsStream())
                        {
                            await stream.WriteAsync(kvp.Value).ConfigureAwait(false);
                        }
                    });
                    }

                    app.UseRouter(route.Build());
                });

            testServer = new TestServer(whb);
            httpClient = testServer.CreateClient();
        }

        [Params("/nuget-org.json", "/dotnet-core.json")]
        public string Uri { get; set; }

        [GlobalCleanup]
        public void Cleanup()
        {
            httpClient?.Dispose();
            testServer?.Dispose();
        }

        [Benchmark(Description = "System.Text.Json JsonSerializer", Baseline = true)]
        public async Task<SearchResults> SystemTextJson()
        {
            var response = await httpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var obj = await Stj.JsonSerializer.DeserializeAsync<SearchResults>(stream);

            return obj;
        }

        [Benchmark(Description = "System.Text.Json with JsonConverters")]
        public async Task<SearchResults> SystemTextJsonCustomConverter()
        {
            var response = await httpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new Converters.Stj.SearchResultsConverter());
            options.Converters.Add(new Converters.Stj.SearchResultConverter());
            options.Converters.Add(new Converters.Stj.SearchResultVersionConverter());

            var obj = await Stj.JsonSerializer.DeserializeAsync<SearchResults>(stream, options: options).ConfigureAwait(false);

            return obj;
        }

        [Benchmark(Description = "Newtonsoft.Json JsonSerializer")]
        public async Task<SearchResults> NewtonsoftJson()
        {
            var response = await httpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            SearchResults obj;
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new Nj.JsonSerializer();
                obj = serializer.Deserialize<SearchResults>(jsonReader);
            }

            return obj;
        }

        [Benchmark(Description = "Newtonsoft.Json with JsonConverters")]
        public async Task<SearchResults> NewtonsoftJsonCustomConverter()
        {
            var response = await httpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            SearchResults obj;
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new Nj.JsonSerializer();
                serializer.Converters.Add(new Converters.Nj.SearchResultsConverter());
                serializer.Converters.Add(new Converters.Nj.SearchResultConverter());
                serializer.Converters.Add(new Converters.Nj.SearchResultVersionConverter());

                obj = serializer.Deserialize<SearchResults>(jsonReader);
            }

            return obj;
        }

        [Benchmark(Description = "Newtonsoft.Json with JTokens")]
        public async Task<SearchResults> NewtonsoftJsonJToken()
        {
            var response = await httpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var jtoken = JToken.ReadFrom(jsonReader);
                var obj = jtoken.ToObject<SearchResults>();
                return obj;
            }
        }
    }
}
