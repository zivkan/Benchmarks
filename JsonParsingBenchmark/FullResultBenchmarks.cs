using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using JsonParsingBenchmark.Converters.Stj;
using JsonParsingBenchmark.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Nj = Newtonsoft.Json;
using Stj = System.Text.Json;

namespace JsonParsingBenchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net80)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net481)]
    public class FullResultBenchmarks
    {
        private JsonSerializerOptions StjOptions;
        private Nj.JsonSerializer NjDefaultSerializer;
        private Nj.JsonSerializer NjConvertersSerializer;
        private IReadOnlyDictionary<string, string> _inputFiles;

        [GlobalSetup]
        public void Setup()
        {
            _inputFiles = Startup.LoadTestData();

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

        [Params("nuget-org.json", "dotnet-core.json")]
        public string InputFile { get; set; }

        [Benchmark(Description = "System.Text.Json JsonSerializer", Baseline = true)]
        public async Task<SearchResults> SystemTextJson()
        {
            using (var stream = File.OpenRead(_inputFiles[InputFile]))
            {
                return await Stj.JsonSerializer.DeserializeAsync<SearchResults>(stream);
            }
        }

        [Benchmark(Description = "System.Text.Json with JsonConverters")]
        public async Task<SearchResults> SystemTextJsonCustomConverter()
        {
            using (var stream = File.OpenRead(_inputFiles[InputFile]))
            {
                return await Stj.JsonSerializer.DeserializeAsync<SearchResults>(stream, options: StjOptions).ConfigureAwait(false);
            }
        }

        [Benchmark(Description = "System.Text.Json with JsonDocument")]
        public SearchResults SystemTextJsonJsonDocument()
        {
            using (var stream = File.OpenRead(_inputFiles[InputFile]))
            using (var cursor = JsonDocument.Parse(stream))
            {
                return SearchResultJsonDocumentReader.Parse(cursor.RootElement);
            }
        }

        [Benchmark(Description = "System.Text.Json with JsonDocument with Cloning")]
        public SearchResults SystemTextJsonJsonDocumentWithCloning()
        {
            JsonElement clonedElement;
            using (var stream = File.OpenRead(_inputFiles[InputFile]))
            using (var cursor = JsonDocument.Parse(stream))
            {
                clonedElement = cursor.RootElement.Clone();
            }
            return SearchResultJsonDocumentReader.Parse(clonedElement);
        }


        [Benchmark(Description = "Newtonsoft.Json JsonSerializer")]
        public SearchResults NewtonsoftJson()
        {
            SearchResults obj;
            using (var streamReader = new StreamReader(_inputFiles[InputFile]))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                obj = NjDefaultSerializer.Deserialize<SearchResults>(jsonReader);
            }

            return obj;
        }

        [Benchmark(Description = "Newtonsoft.Json with JsonConverters")]
        public SearchResults NewtonsoftJsonCustomConverter()
        {
            SearchResults obj;
            using (var streamReader = new StreamReader(_inputFiles[InputFile]))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                obj = NjConvertersSerializer.Deserialize<SearchResults>(jsonReader);
            }

            return obj;
        }

        [Benchmark(Description = "Newtonsoft.Json with JTokens")]
        public SearchResults NewtonsoftJsonJToken()
        {
            using (var streamReader = new StreamReader(_inputFiles[InputFile]))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var jtoken = JToken.ReadFrom(jsonReader);
                var obj = jtoken.ToObject<SearchResults>();
                return obj;
            }
        }
    }
}
