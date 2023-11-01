using Microsoft.IO;
using Newtonsoft.Json;
using NuGetCatalogParserBenchmark.NuGetModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NuGetCatalogParserBenchmark
{
    public class Mirror
    {
        private readonly string _localPath;
        private readonly HttpClient _client;
        private readonly RecyclableMemoryStreamManager _memoryStreamManager;
        private readonly JsonSerializer _jsonSerializer;

        private int _checked;
        private int _downloaded;

        public int Checked { get => _checked; }
        public int Downlaoded { get => _downloaded; }

        public Mirror(string localPath)
        {
            _localPath = localPath;
            _client = new HttpClient();
            _memoryStreamManager = new RecyclableMemoryStreamManager();
            _jsonSerializer = new JsonSerializer();
        }

        public async Task Run(bool update)
        {
            ServiceIndex serviceIndex = await GetServiceIndexAsync();
            var catalogUrl = serviceIndex.Resources.Single(r => string.Equals(r.Type, "Catalog/3.0.0", StringComparison.OrdinalIgnoreCase)).Id;
            var catalogIndex = await GetCatalogIndexAsync(catalogUrl, update);

            const int maxConcurrent = 200;
            ServicePointManager.DefaultConnectionLimit = maxConcurrent;
            var tasks = new List<Task>(maxConcurrent);

            for (int i = 0; i < catalogIndex.Items.Count; i++)
            {
                if (tasks.Count >= maxConcurrent)
                {
                    var finished = await Task.WhenAny(tasks);
                    tasks.Remove(finished);
                }

                var catalogPage = catalogIndex.Items[i];
                tasks.Add(Task.Run(() => DownloadCatalogPage(catalogPage, update)));
            }

            await Task.WhenAll(tasks);
        }

        private async Task<ServiceIndex> GetServiceIndexAsync()
        {
            var response = await _client.GetAsync("https://api.nuget.org/v3/index.json");
            var serviceIndex = await response.Content.ReadFromJsonAsync<ServiceIndex>();
            return serviceIndex;
        }

        private async Task<CatalogIndex> GetCatalogIndexAsync(string catalogUrl, bool update)
        {
            var indexFileName = Path.Combine(_localPath, "index.json");

            if (update || !File.Exists(indexFileName))
            {
                var response = await _client.GetAsync(catalogUrl);
                using (var destination = File.Create(indexFileName))
                {
                    await response.Content.CopyToAsync(destination);
                }
            }

            CatalogIndex catalogIndex;
            using (var streamReader = new StreamReader(indexFileName))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                catalogIndex = _jsonSerializer.Deserialize<CatalogIndex>(jsonReader);
            }

            return catalogIndex;
        }

        private async Task DownloadCatalogPage(CatalogIndex.Item item, bool update)
        {
            var uri = new Uri(item.Id);
            var fileName = Path.Combine(_localPath, uri.Segments[uri.Segments.Length - 1]);
            var download = true;
            if (File.Exists(fileName))
            {
                try
                {
                    var page = await Utility.ReadAsync<CatalogPage>(fileName, _jsonSerializer, _memoryStreamManager);

                    if (!update ||
                        (page.CommitId == item.CommitId &&
                        page.CommitTimeStamp == item.CommitTimeStamp &&
                        page.Count == item.Count))
                    {
                        download = false;
                    }
                }
                catch
                {
                    download = true;
                }
            }

            if (download)
            {
                var response = await _client.GetAsync(uri);

                using (var fileStream = File.Create(fileName))
                {
                    await response.Content.CopyToAsync(fileStream);
                }

                Interlocked.Increment(ref _downloaded);
            }

            Interlocked.Increment(ref _checked);
        }
    }
}
