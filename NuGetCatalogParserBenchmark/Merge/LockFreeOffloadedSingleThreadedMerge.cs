using Microsoft.IO;
using Newtonsoft.Json;
using NuGetCatalogParserBenchmark.NuGetModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NuGetCatalogParserBenchmark.Merge
{
    public class LockFreeOffloadedSingleThreadedMerge
    {
        private readonly string _cachePath;
        private readonly int _maxConcurrent;
        private readonly JsonSerializer _jsonSerializer;
        private readonly RecyclableMemoryStreamManager _memoryStreamManager;

        public LockFreeOffloadedSingleThreadedMerge(string cachePath, int maxConcurrent)
        {
            _cachePath = cachePath;
            _maxConcurrent = maxConcurrent;
            _jsonSerializer = new JsonSerializer();
            _memoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task<Dictionary<string, List<string>>> Go()
        {
            var index = Utility.Read<CatalogIndex>(Path.Combine(_cachePath, "index.json"), _jsonSerializer);

            Dictionary<string, Dictionary<string, (DateTime timeStamp, bool deleted)>> dest = null;
            var maxConcurrent = Math.Min(_maxConcurrent, index.Items.Count);
            var tasks = new List<Task<Dictionary<string, Dictionary<string, (DateTime timeStamp, bool deleted)>>>>(maxConcurrent);
            int pageIndex;
            for (pageIndex = 0; pageIndex < maxConcurrent; pageIndex++)
            {
                var uri = index.Items[pageIndex].Id;
                tasks.Add(Task.Run(() => ProcessPage(uri)));
            }

            while (tasks.Count > 0)
            {
                var finished = await Task.WhenAny(tasks);
                if (!finished.IsCompletedSuccessfully)
                {
                    await finished;
                }
                tasks.Remove(finished);
                if (pageIndex < index.Items.Count)
                {
                    var uri = index.Items[pageIndex].Id;
                    tasks.Add(Task.Run(() => ProcessPage(uri)));
                    pageIndex++;
                }

                var src = finished.Result;
                if (dest == null)
                {
                    dest = src;
                }
                else
                {
                    Utility.Merge(dest, src);
                }
            }

            var result = Utility.ConvertIntermediateResultToFinal(dest);
            return result;
        }

        private async Task<Dictionary<string, Dictionary<string, (DateTime timeStamp, bool deleted)>>> ProcessPage(string url)
        {
            var uri = new Uri(url);
            var fileName = Path.Combine(_cachePath, uri.Segments[uri.Segments.Length - 1]);
            var page = await Utility.ReadAsync<CatalogPage>(fileName, _jsonSerializer, _memoryStreamManager);

            var dict = Utility.ConvertCatalogPageItemsToIntermediate(page.Items);

            return dict;
        }
    }
}
