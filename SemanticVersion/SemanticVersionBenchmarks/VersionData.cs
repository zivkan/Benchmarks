using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using zivkan.Benchmarks.Common;
using zivkan.Benchmarks.Common.NuGet.Protocol.Catalog;

namespace SemanticVersionBenchmarks
{
    public static class VersionData
    {
        public static async Task<List<string>> GetVersionsAsync(bool update = false)
        {
            var baseCachePath = TestData.GetCachePath();
            var versionsFileDirectory = Path.Combine(baseCachePath, nameof(SemanticVersionBenchmarks));
            var versionsFilePath = Path.Combine(versionsFileDirectory, "versions.json");

            if (update || !File.Exists(versionsFilePath))
            {
                var mirror = new Mirror(baseCachePath);
                await mirror.MirrorNuGetOrgCatalog(update);

                var versions = await ReadAllVersionsFromCatalogAsync(baseCachePath);

                if (!Directory.Exists(versionsFileDirectory))
                {
                    Directory.CreateDirectory(versionsFileDirectory);
                }

                using (var fileStream = File.Create(versionsFilePath))
                {
                    var options = new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    };
                    await JsonSerializer.SerializeAsync(fileStream, versions, options);
                }

                return versions;
            }
            else
            {
                using (var fileStream = File.OpenRead(versionsFilePath))
                {
                    var versions = await JsonSerializer.DeserializeAsync<List<string>>(fileStream);
                    if (versions == null)
                    {
                        throw new Exception("Deserialized versions file is null");
                    }
                    return versions;
                }
            }
        }

        private static async Task<List<string>> ReadAllVersionsFromCatalogAsync(string baseCachePath)
        {
            var catalogDirectory = Path.Combine(baseCachePath, TestData.NuGetOrgCatalog);

            CatalogIndex catalogIndex;
            using (var fileStream = File.OpenRead(Path.Combine(catalogDirectory, "index.json")))
            {
                CatalogIndex? deserialized = await JsonSerializer.DeserializeAsync<CatalogIndex>(fileStream);
                catalogIndex = deserialized ?? throw new Exception("Deserialized catalog index is null");
            }

            var pageChannel = Channel.CreateUnbounded<string>();
            var versionsChannel = Channel.CreateUnbounded<HashSet<string>>();

            var pageTasks = new Task[Environment.ProcessorCount];
            for (int i = 0; i < pageTasks.Length; i++)
            {
                pageTasks[i] = GetVersionsFromFileAsync(pageChannel.Reader, catalogDirectory, versionsChannel.Writer);
            }

            HashSet<string> allVersions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Task aggregateVersionsTask = AggregateVersionsAsync(versionsChannel.Reader, allVersions);

            if (catalogIndex.items == null)
            {
                throw new Exception("Catalog index has null items");
            }

            foreach (var catalogItem in catalogIndex.items)
            {
                if (catalogItem.id == null)
                {
                    throw new Exception("Catalog item's ID is null");
                }

                await pageChannel.Writer.WriteAsync(catalogItem.id);
            }

            pageChannel.Writer.Complete();
            await Task.WhenAll(pageTasks);
            versionsChannel.Writer.Complete();
            await aggregateVersionsTask;

            var versions = new List<string>(allVersions.Count);
            foreach (var version in allVersions.OrderBy(v => v))
            {
                versions.Add(version);
            }

            return versions;
        }

        private static async Task AggregateVersionsAsync(ChannelReader<HashSet<string>> reader, HashSet<string> allVersions)
        {
            try
            {
                HashSet<string> next;
                while ((next = await reader.ReadAsync()) != null)
                {
                    foreach (var version in next)
                    {
                        allVersions.Add(version);
                    }
                }
            }
            catch (ChannelClosedException)
            {
            }
        }

        private static async Task GetVersionsFromFileAsync(ChannelReader<string> reader, string cachePath, ChannelWriter<HashSet<string>> writer)
        {
            string pageUri;
            try
            {
                while ((pageUri = await reader.ReadAsync()) != null)
                {
                    var uri = new Uri(pageUri);
                    var filePath = Path.Combine(cachePath, uri.Segments[uri.Segments.Length - 1]);
                    CatalogPage page;
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        var deserialized = await JsonSerializer.DeserializeAsync<CatalogPage>(fileStream);
                        page = deserialized ?? throw new Exception("Page is null");
                    }

                    if (page.items == null)
                    {
                        throw new Exception(pageUri + " has null items");
                    }

                    HashSet<string> versions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in page.items)
                    {
                        if (string.IsNullOrEmpty(item.packageVersion))
                        {
                            throw new Exception("Package version is null or empty");
                        }

                        versions.Add(item.packageVersion!);
                    }

                    await writer.WriteAsync(versions);
                }
            }
            catch (ChannelClosedException)
            {
            }
        }
    }
}
