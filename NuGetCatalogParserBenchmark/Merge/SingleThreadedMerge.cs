using Newtonsoft.Json;
using NuGetCatalogParserBenchmark.NuGetModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NuGetCatalogParserBenchmark.Merge
{
    public class SingleThreadedMerge
    {
        private readonly string _cachePath;
        private readonly JsonSerializer _jsonSerializer;

        public SingleThreadedMerge(string cachePath)
        {
            _cachePath = cachePath;
            _jsonSerializer = new JsonSerializer();
        }

        public Dictionary<string, List<string>> Go()
        {
            var index = Utility.Read<CatalogIndex>(Path.Combine(_cachePath, "index.json"), _jsonSerializer);

            var dest = new Dictionary<string, Dictionary<string, (DateTime timeStamp, bool deleted)>>();

            for (int i = 0; i < index.Count; i++)
            {
                var uri = new Uri(index.Items[i].Id);
                var fileName = Path.Combine(_cachePath, uri.Segments[uri.Segments.Length - 1]);
                var page = Utility.Read<CatalogPage>(fileName, _jsonSerializer);

                foreach (var src in page.Items)
                {
                    if (!dest.TryGetValue(src.PackageId, out var versions))
                    {
                        versions = new Dictionary<string, (DateTime timeStamp, bool deleted)>();
                        dest.Add(src.PackageId, versions);
                    }

                    var itemDateTime = DateTime.Parse(src.CommitTimeStamp, null, DateTimeStyles.AdjustToUniversal);
                    if (versions.TryGetValue(src.PackageVersion, out var dateTime))
                    {
                        if (dateTime.Item1 < itemDateTime)
                        {
                            bool deleted = src.Type == "nuget:PackageDelete";
                            versions[src.PackageVersion] = (itemDateTime, deleted);
                        }
                    }
                    else
                    {
                        bool deleted = src.Type == "nuget:PackageDelete";
                        versions.Add(src.PackageVersion, (itemDateTime, deleted));
                    }
                }
            }

            var result = Utility.ConvertIntermediateResultToFinal(dest);
            return result;
        }
    }
}
