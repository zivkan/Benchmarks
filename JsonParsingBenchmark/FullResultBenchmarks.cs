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
        private JsonSerializerOptions StjOptions;
        private Nj.JsonSerializer NjDefaultSerializer;
        private Nj.JsonSerializer NjConvertersSerializer;


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

            StjOptions = new JsonSerializerOptions();
            StjOptions.Converters.Add(new Converters.Stj.SearchResultsConverter());
            StjOptions.Converters.Add(new Converters.Stj.SearchResultConverter());
            StjOptions.Converters.Add(new Converters.Stj.SearchResultVersionConverter());

            NjDefaultSerializer = new Nj.JsonSerializer();

            NjConvertersSerializer = new Nj.JsonSerializer();
            NjConvertersSerializer.Converters.Add(new Converters.Nj.SearchResultsConverter());
            NjConvertersSerializer.Converters.Add(new Converters.Nj.SearchResultConverter());
            NjConvertersSerializer.Converters.Add(new Converters.Nj.SearchResultVersionConverter());
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

            var obj = await Stj.JsonSerializer.DeserializeAsync<SearchResults>(stream, options: StjOptions).ConfigureAwait(false);

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
                obj = NjDefaultSerializer.Deserialize<SearchResults>(jsonReader);
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
                obj = NjConvertersSerializer.Deserialize<SearchResults>(jsonReader);
            }

            return obj;
        }

        [Benchmark(Description = "Newtonsoft.Json with JTokens")]
        public async Task<SearchResults> NewtonsoftJsonJToken()
        {
            // https://github.com/NuGet/NuGet.Client/blob/5d1af18e560b5f381626b5a7e9ce2acacb6211db/src/NuGet.Core/NuGet.Protocol/HttpSource/HttpRetryHandler.cs#L90
            // https://github.com/NuGet/NuGet.Client/blob/3803820961f4d61c06d07b179dab1d0439ec0d91/src/NuGet.Core/NuGet.Protocol/HttpSource/HttpRetryHandlerRequest.cs#L23
            var response = await httpClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            // https://github.com/NuGet/NuGet.Client/blob/3803820961f4d61c06d07b179dab1d0439ec0d91/src/NuGet.Core/NuGet.Protocol/HttpSource/HttpRetryHandler.cs#L98
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            // https://github.com/NuGet/NuGet.Client/blob/5d1af18e560b5f381626b5a7e9ce2acacb6211db/src/NuGet.Core/NuGet.Protocol/Utility/StreamExtensions.cs#L66-L73
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
            memoryStream.Position = 0;

            // https://github.com/NuGet/NuGet.Client/blob/007e5c55cad83aa90733ede04eec68a8c29f9be8/src/NuGet.Core/NuGet.Protocol/HttpSource/HttpSource.cs#L271
            // https://github.com/NuGet/NuGet.Client/blob/5d1af18e560b5f381626b5a7e9ce2acacb6211db/src/NuGet.Core/NuGet.Protocol/Utility/StreamExtensions.cs#L28-L31
            // https://github.com/NuGet/NuGet.Client/blob/5d1af18e560b5f381626b5a7e9ce2acacb6211db/src/NuGet.Core/NuGet.ProjectModel/JsonUtility.cs#L36
            using (var streamReader = new StreamReader(memoryStream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var jtoken = JToken.ReadFrom(jsonReader);
                // https://github.com/NuGet/NuGet.Client/blob/8058f199bdcce2fddaf35a19474b9ab69f4b4a34/src/NuGet.Core/NuGet.Protocol/Resources/PackageSearchResourceV3.cs#L29
                var obj = jtoken.ToObject<SearchResults>();
                return obj;
            }
        }
    }
}
