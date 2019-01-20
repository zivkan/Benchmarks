using Microsoft.IO;
using Newtonsoft.Json;
using NuGetCatalogParserBenchmark.NuGetModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NuGetCatalogParserBenchmark
{
    public static class Utility
    {
        public static string GetCachePath()
        {
            var directory = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var gitignore = Path.Combine(directory, ".gitignore");
            while (!File.Exists(gitignore))
            {
                var newDir = Path.GetDirectoryName(directory);
                if (newDir == directory)
                {
                    throw new FileNotFoundException("Can't find project's readme.md file.");
                }
                directory = newDir;
                gitignore = Path.Combine(directory, ".gitignore");
            }

            var testData = Path.Combine(directory, "TestData", "NuGetCatalogParserBenchmark");
            return testData;
        }

        public static async Task<T> ReadAsync<T>(string fileName, JsonSerializer jsonSerializer, RecyclableMemoryStreamManager memoryStreamManager)
        {
            using (var memoryStream = memoryStreamManager.GetStream())
            {
                using (var reader = File.OpenRead(fileName))
                {
                    await reader.CopyToAsync(memoryStream);
                }

                memoryStream.Position = 0;
                using (var streamReader = new StreamReader(memoryStream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    return jsonSerializer.Deserialize<T>(jsonReader);
                }
            }
        }

        public static T Read<T>(string fileName, JsonSerializer jsonSerializer)
        {
            using (var reader = new StreamReader(fileName))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return jsonSerializer.Deserialize<T>(jsonReader);
            }
        }

        internal static Dictionary<string, Dictionary<string, (DateTime timeStamp, bool deleted)>>
            ConvertCatalogPageItemsToIntermediate(List<CatalogPage.Item> items)
        {
            var dict = new Dictionary<string, Dictionary<string, (DateTime timeStamp, bool deleted)>>();

            foreach (var item in items)
            {
                if (!dict.TryGetValue(item.PackageId, out var versions))
                {
                    versions = new Dictionary<string, (DateTime timeStamp, bool deleted)>();
                    dict.Add(item.PackageId, versions);
                }

                var itemDateTime = DateTime.Parse(item.CommitTimeStamp, null, DateTimeStyles.AdjustToUniversal);
                if (versions.TryGetValue(item.PackageVersion, out var dateTime))
                {
                    if (dateTime.Item1 < itemDateTime)
                    {
                        bool deleted = item.Type == "nuget:PackageDelete";
                        versions[item.PackageVersion] = (itemDateTime, deleted);
                    }
                }
                else
                {
                    bool deleted = item.Type == "nuget:PackageDelete";
                    versions.Add(item.PackageVersion, (itemDateTime, deleted));
                }
            }

            return dict;
        }

        public static void Merge(
            Dictionary<string, Dictionary<string, (DateTime timeStamp, bool deleted)>> dest, 
            Dictionary<string, Dictionary<string, (DateTime timeStamp, bool deleted)>> src)
        {
            foreach (var srcPackage in src)
            {
                if (dest.TryGetValue(srcPackage.Key, out var destVersions))
                {
                    foreach (var srcVersion in srcPackage.Value)
                    {
                        if (destVersions.TryGetValue(srcVersion.Key, out var destVersion))
                        {
                            if (srcVersion.Value.timeStamp > destVersion.timeStamp)
                            {
                                destVersions[srcVersion.Key] = srcVersion.Value;
                            }
                        }
                        else
                        {
                            destVersions.Add(srcVersion.Key, srcVersion.Value);
                        }
                    }
                }
                else
                {
                    dest.Add(srcPackage.Key, srcPackage.Value);
                }
            }
        }

        public static Dictionary<string, List<string>> ConvertIntermediateResultToFinal<T1, T2>(T1 intermediate)
            where T1 : IDictionary<string, T2>
            where T2 : IDictionary<string, (DateTime timeStamp, bool deleted)>
        {
            var final = new Dictionary<string, List<string>>();

            var packageIds = intermediate.Keys.OrderBy(k => k);
            foreach (var packageId in packageIds)
            {
                var versions = intermediate[packageId].Where(v => !v.Value.deleted).Select(v => v.Key).OrderBy(v => v).ToList();
                if (versions.Count > 0)
                {
                    final.Add(packageId, versions);
                }
            }

            return final;
        }

        public static Dictionary<string, List<string>> ConvertIntermediateResultToFinal(Dictionary<string, Dictionary<string, (DateTime, bool)>> intermediate)
        {
            return ConvertIntermediateResultToFinal<Dictionary<string, Dictionary<string, (DateTime, bool)>>, Dictionary<string, (DateTime, bool)>>(intermediate);
        }

        public static Dictionary<string, List<string>> ConvertIntermediateResultToFinal(ConcurrentDictionary<string, ConcurrentDictionary<string, (DateTime, bool)>> intermediate)
        {
            return ConvertIntermediateResultToFinal<ConcurrentDictionary<string, ConcurrentDictionary<string, (DateTime, bool)>>, ConcurrentDictionary<string, (DateTime, bool)>>(intermediate);
        }
    }
}
