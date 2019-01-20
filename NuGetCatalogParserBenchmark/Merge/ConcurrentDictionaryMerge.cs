using Microsoft.IO;
using Newtonsoft.Json;
using NuGetCatalogParserBenchmark.NuGetModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace NuGetCatalogParserBenchmark.Merge
{
    public class ConcurrentDictionaryMerge
    {
        private readonly string _cachePath;
        private readonly JsonSerializer _jsonSerializer;
        private readonly RecyclableMemoryStreamManager _memoryStreamManager;
        private ConcurrentDictionary<string, ConcurrentDictionary<string, (DateTime timeStamp, bool deleted)>> _dictionary;

        public ConcurrentDictionaryMerge(string cachePath)
        {
            _cachePath = cachePath;
            _jsonSerializer = new JsonSerializer();
            _memoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task<Dictionary<string, List<string>>> Go()
        {
            _dictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, (DateTime timeStamp, bool deleted)>>();

            var indexFileName = Path.Combine(_cachePath, "index.json");
            var index = await Utility.ReadAsync<CatalogIndex>(indexFileName, _jsonSerializer, _memoryStreamManager);

            int maxConcurrent = Environment.ProcessorCount;
            var tasks = new List<Task>(maxConcurrent);

            for (int i = 0; i < index.Items.Count; i++)
            {
                if (tasks.Count >= maxConcurrent)
                {
                    var finished = await Task.WhenAny(tasks);
                    if (!finished.IsCompletedSuccessfully)
                    {
                        await finished;
                    }
                    tasks.Remove(finished);
                }

                var uri = index.Items[i].Id;
                tasks.Add(Task.Run(() => ProcessPage(uri)));
            }

            await Task.WhenAll(tasks);

            var result = Utility.ConvertIntermediateResultToFinal(_dictionary);
            return result;
        }

        private void ProcessPage(string uri)
        {
            var u = new Uri(uri);
            var fileName = Path.Combine(_cachePath, u.Segments[u.Segments.Length - 1]);
            var page = Utility.Read<CatalogPage>(fileName, _jsonSerializer);

            foreach (var item in page.Items)
            {
                var timeStamp = DateTime.Parse(item.CommitTimeStamp, null, DateTimeStyles.AdjustToUniversal);
                bool deleted = item.Type == "nuget:PackageDelete";
                _dictionary.AddOrUpdate(item.PackageId,
                    key =>
                    {
                        var versions = new ConcurrentDictionary<string, (DateTime, bool)>();
                        versions[item.PackageVersion] = (timeStamp, deleted);
                        return versions;
                    },
                    (key, versions) =>
                    {
                        versions.AddOrUpdate(item.PackageVersion,
                            verion => (timeStamp, deleted),
                            (version, latest) => latest.timeStamp > timeStamp ? latest : (timeStamp, deleted));
                        return versions;
                    });
            }
        }
    }
}
