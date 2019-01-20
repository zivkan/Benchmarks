using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGetCatalogParserTests
{
    internal static class ResultComparer
    {
        public static void Compare(Dictionary<string, List<string>> expected, Dictionary<string, List<string>> actual)
        {
            var missing = new HashSet<string>(expected.Count);
            var extra = new HashSet<string>();
            var common = new HashSet<string>();
            var sb = new StringBuilder();

            foreach (var key in expected.Keys)
            {
                missing.Add(key);
            }

            foreach (var key in actual.Keys)
            {
                if (missing.Contains(key))
                {
                    missing.Remove(key);
                    common.Add(key);
                }
                else
                {
                    extra.Add(key);
                }
            }

            if (missing.Count > 0)
            {
                sb.Append("Missing packages ");
                foreach (var id in missing.OrderBy(p => p))
                {
                    sb.AppendFormat("{0}, ", id);
                }
                sb.Length -= 2;
                sb.AppendLine();
            }

            if (extra.Count > 0)
            {
                sb.Append("Extra packages ");
                foreach (var id in extra.OrderBy(p => p))
                {
                    sb.AppendFormat("{0}, ", id);
                }
                sb.Length -= 2;
                sb.AppendLine();
            }

            int goodPackages = 0;
            foreach (var package in common)
            {
                missing.Clear();
                extra.Clear();
                foreach (var version in expected[package]) missing.Add(version);

                foreach(var version in actual[package])
                {
                    if (missing.Contains(version))
                    {
                        missing.Remove(version);
                    }
                    else
                    {
                        extra.Add(version);
                    }
                }

                if (missing.Count == 0 && extra.Count == 0)
                {
                    goodPackages++;
                }
                else
                {
                    sb.AppendFormat("Package {0}", package);
                    if (missing.Count > 0)
                    {
                        sb.Append(" missing version(s) ");
                        foreach (var version in missing) sb.AppendFormat("{0}, ", version);
                        sb.Length -= 2;
                    }
                    if (extra.Count > 0)
                    {
                        sb.Append(" extra versions(s) ");
                        foreach (var version in extra) sb.AppendFormat("{0}, ", version);
                        sb.Length -= 2;
                    }
                    sb.AppendLine();
                }
            }

            if (sb.Length > 0)
            {
                sb.AppendFormat("{0} correct packages", goodPackages);
                throw new Exception(sb.ToString());
            }
        }
    }
}
