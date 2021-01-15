using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using zivkan.Benchmarks.Common.NuGet.Protocol;
using zivkan.Benchmarks.Common.NuGet.Protocol.Catalog;

namespace zivkan.Benchmarks.Common
{
    public class Mirror
    {
        private readonly string _localPath;
        private readonly HttpClient _client;

        private int _checked;
        private int _downloaded;

        public int Checked { get => _checked; }
        public int Downlaoded { get => _downloaded; }

        public Mirror(string baseTestDataDirectory)
        {
            _localPath = baseTestDataDirectory;

            var handler = new HttpClientHandler();
            handler.MaxConnectionsPerServer = 20;

            _client = new HttpClient();
        }

        public async Task MirrorNuGetOrgCatalog(bool update)
        {
            ServiceIndex serviceIndex = await GetServiceIndexAsync();
            var catalogUrl = serviceIndex.Resources?.Single(r => string.Equals(r.Type, "Catalog/3.0.0", StringComparison.OrdinalIgnoreCase)).Id;
            if (catalogUrl == null)
            {
                throw new Exception("Could not find catalog URL");
            }
            var catalogIndex = await GetCatalogIndexAsync(catalogUrl, update);
            if (catalogIndex.items == null)
            {
                throw new Exception("Catalog index's items is null");
            }

            const int maxConcurrent = 200;
            var tasks = new List<Task>(maxConcurrent);

            for (int i = 0; i < catalogIndex.items.Count; i++)
            {
                if (tasks.Count >= maxConcurrent)
                {
                    var finished = await Task.WhenAny(tasks);
                    tasks.Remove(finished);
                }

                var catalogPage = catalogIndex.items[i];
                tasks.Add(Task.Run(() => DownloadCatalogPage(catalogPage)));
            }

            await Task.WhenAll(tasks);
        }

        private async Task<ServiceIndex> GetServiceIndexAsync()
        {
            var response = await _client.GetAsync("https://api.nuget.org/v3/index.json");
            var serviceIndex = await response.Content.ReadFromJsonAsync<ServiceIndex>();
            if (serviceIndex == null)
            {
                throw new Exception("Service index is null");
            }
            return serviceIndex;
        }

        private async Task<CatalogIndex> GetCatalogIndexAsync(string catalogUrl, bool update)
        {
            var cachePath = Path.Combine(_localPath, TestData.NuGetOrgCatalog);
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var indexFileName = Path.Combine(cachePath, "index.json");

            if (update || !File.Exists(indexFileName))
            {
                var response = await _client.GetAsync(catalogUrl);
                using (var destination = File.Create(indexFileName))
                {
                    await response.Content.CopyToAsync(destination);
                }
            }

            CatalogIndex? catalogIndex;
            using (var fileReader = File.OpenRead(indexFileName))
            {
                catalogIndex = await JsonSerializer.DeserializeAsync<CatalogIndex>(fileReader);
            }

            if (catalogIndex == null)
            {
                throw new Exception("Catalog index is null");
            }

            return catalogIndex;
        }

        private async Task DownloadCatalogPage(CatalogIndexItem item)
        {
            if (item.id == null)
            {
                throw new ArgumentException(message: "Id property must not be null", paramName: nameof(item));
            }

            var uri = new Uri(item.id);
            var fileName = Path.Combine(_localPath, TestData.NuGetOrgCatalog, uri.Segments[uri.Segments.Length]);
            var download = true;
            if (File.Exists(fileName))
            {
                try
                {
                    CatalogPage? page;
                    using (var fileStream = File.OpenRead(fileName))
                    {
                         page = await JsonSerializer.DeserializeAsync<CatalogPage>(fileStream);
                        if (page == null)
                        {
                            throw new Exception("Catalog page is null");
                        }
                    }

                    // If the cached page matches the catalog index's metadata, or it's a page with a known issue, don't download
                    if (Equals(page, item) || IsKnownProblem(page, item))
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

        private static bool Equals(CatalogPage page, CatalogIndexItem item)
        {
            return page.commitId == item.commitId
                && page.commitTimeStamp == item.commitTimeStamp
                && page.count == item.count;
        }

        // https://github.com/NuGet/NuGetGallery/issues/6827
        private static bool IsKnownProblem(CatalogPage page, CatalogIndexItem item)
        {
            foreach (var knownProblem in _knownProblems)
            {
                if (item.id == knownProblem.Id
                    && item.commitTimeStamp == knownProblem.IndexTimeStamp
                    && item.count == knownProblem.IndexCount
                    && item.commitId == knownProblem.IndexCommitId
                    && page.commitTimeStamp == knownProblem.PageTimeStamp
                    && page.count == knownProblem.PageCount
                    && page.commitId == knownProblem.PageCommitId)
                {
                    return true;
                }
            }
            return false;
        }

        private class KnownProblem
        {
            public string Id { get; }
            public int IndexCount { get; }
            public string IndexTimeStamp { get; }
            public string IndexCommitId { get; }
            public int PageCount { get; }
            public string PageTimeStamp { get; }
            public string PageCommitId { get; }

            public KnownProblem(string id, int indexCount, string indexTimeStamp, string indexCommitId, int pageCount, string pageTimeStamp, string pageCommitId)
            {
                Id = id;
                IndexCount = indexCount;
                IndexTimeStamp = indexTimeStamp;
                IndexCommitId = indexCommitId;
                PageCount = pageCount;
                PageTimeStamp = pageTimeStamp;
                PageCommitId = pageCommitId;
            }
        }

        private static readonly IReadOnlyList<KnownProblem> _knownProblems = new[]
        {
            new KnownProblem("https://api.nuget.org/v3/catalog0/page1300.json", 549, "2016-01-13T22:11:37.7649356Z", "7c51a2eb-dc87-40a8-831b-83469b2526d7", 550, "2016-01-13T22:11:49.1579762Z", "dae05307-06a5-4954-aeaf-567b88c6b266"),
            new KnownProblem("https://api.nuget.org/v3/catalog0/page1309.json", 548, "2016-01-15T04:02:48.8858301Z", "02842ef9-7e5c-4df5-9dad-fa09bf7675da", 550, "2016-01-15T04:02:56.9796327Z", "ac9f13d7-bd08-4df5-b562-994a9ff0df18"),
            new KnownProblem("https://api.nuget.org/v3/catalog0/page1313.json", 550, "2016-01-15T16:18:09.8452301Z", "c652a5a3-a0ad-4986-bcf3-229aa33af5b0", 552, "2016-01-15T16:18:13.8407352Z", "3c10fced-9fd1-4ecc-a9a9-ecba436a454b"),
            new KnownProblem("https://api.nuget.org/v3/catalog0/page1367.json", 550, "2016-02-11T10:00:49.8705942Z", "a6fb5dc0-ab0d-4d6d-9f66-65f1c7b0e188", 551, "2016-02-11T10:00:55.9548404Z", "b4376f7c-98f0-4a69-96df-975b8d083379"),
            new KnownProblem("https://api.nuget.org/v3/catalog0/page1436.json", 548, "2016-03-12T07:06:56.3828123Z", "bb0c5083-a8cd-4bba-b49e-d877b58bf853", 549, "2016-03-12T07:07:06.3777682Z", "12a57072-5065-44be-b872-24564369aff1"),
        };
    }
}
