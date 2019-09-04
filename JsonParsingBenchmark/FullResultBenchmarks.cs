using BenchmarkDotNet.Attributes;
using JsonParsingBenchmark.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
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

        [Benchmark]
        public async Task<SearchResults> Stj1()
        {
            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var pipe = PipeReader.Create(stream);
            var readResult = await pipe.ReadAsync();
            while (!readResult.IsCompleted)
            {
                pipe.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
                readResult = await pipe.ReadAsync();
            }

            SearchResults ReadJson(ReadOnlySequence<byte> buffer)
            {
                var jsonReader = new Utf8JsonReader(buffer);
                var obj = JsonSerializer.Deserialize<SearchResults>(ref jsonReader);
                return obj;
            }

            var obj = ReadJson(readResult.Buffer);
            return obj;
        }

        [Benchmark]
        public async Task<SearchResults> Nj1()
        {
            var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var serializer = new Newtonsoft.Json.JsonSerializer();
            SearchResults obj;
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
            {
                obj = serializer.Deserialize<SearchResults>(jsonReader);
            }

            if (obj == null) throw new System.NullReferenceException();

            return obj;
        }
    }
}
