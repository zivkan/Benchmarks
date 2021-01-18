using BenchmarkDotNet.Attributes;
using NuGet.Versioning;
using SemanticVersionBenchmarks.Implementations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SemanticVersionBenchmarks
{
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.NetCoreApp50)]
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net472)]
    [MarkdownExporterAttribute.GitHub]
    public class SortBenchmarks
    {
        private IList _versions;

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            var versions = await VersionData.GetVersionsAsync();

            switch (Implementation)
            {
                case Implementations.NuGet:
                    {
                        var input = new List<NuGetVersion>(versions.Count);
                        for (int i = 0; i < versions.Count; i++)
                        {
                            input.Add(NuGetVersion.Parse(versions[i]));
                        }

                        input.Sort(VersionComparer.Default);
                        _versions = input;
                    }
                    break;

                case Implementations.VersionWithString:
                    {
                        var input = new List<VersionWithStringArray>(versions.Count);
                        for (int i = 0; i < versions.Count; i++)
                        {
                            input.Add(new VersionWithStringArray(versions[i]));
                        }

                        input.Sort(VersionWithStringArray.Compare);
                        _versions = input;
                    }
                    break;

                case Implementations.VersionWithClass:
                    {
                        var input = new List<VersionWithClassArray>(versions.Count);
                        for (int i = 0; i < versions.Count; i++)
                        {
                            input.Add(new VersionWithClassArray(versions[i]));
                        }

                        input.Sort(VersionWithClassArray.Compare);
                        _versions = input;
                    }
                    break;

                case Implementations.VersionWithStruct:
                    {
                        var input = new List<VersionWithStructArray>(versions.Count);
                        for (int i = 0; i < versions.Count; i++)
                        {
                            input.Add(new VersionWithStructArray(versions[i]));
                        }

                        input.Sort(VersionWithStructArray.Compare);
                        _versions = input;
                    }
                    break;

                case Implementations.VersionWithTwoArrays:
                    {
                        var input = new List<VersionWithTwoArrays>(versions.Count);
                        for (int i = 0; i < versions.Count; i++)
                        {
                            input.Add(new VersionWithTwoArrays(versions[i]));
                        }

                        input.Sort(VersionWithTwoArrays.Compare);
                        _versions = input;
                    }
                    break;

                default:
                    throw new Exception("Unkown implementation " + Implementation);
            }
        }

        public enum Implementations
        {
            NuGet,
            VersionWithString,
            VersionWithClass,
            VersionWithStruct,
            VersionWithTwoArrays
        }

        [ParamsAllValues]
        public Implementations Implementation;

        [IterationSetup]
        public void IterationSetup()
        {
            var rand = new Random(1);
            for (int i = _versions.Count - 1; i > 0; i--)
            {
                var index = rand.Next(0, i + 1);
                if (index != i)
                {
                    var temp = _versions[i];
                    _versions[i] = _versions[index];
                    _versions[index] = temp;
                }
            }
        }

        [Benchmark]
        public void Sort()
        {
            switch (Implementation)
            {
                case Implementations.NuGet:
                    {
                        var data = (List<NuGetVersion>)_versions;
                        data.Sort(VersionComparer.Default);
                    }
                    break;

                case Implementations.VersionWithString:
                    {
                        var data = (List<VersionWithStringArray>)_versions;
                        data.Sort(VersionWithStringArray.Compare);
                    }
                    break;

                case Implementations.VersionWithClass:
                    {
                        var data = (List<VersionWithClassArray>)_versions;
                        data.Sort(VersionWithClassArray.Compare);
                    }
                    break;

                case Implementations.VersionWithStruct:
                    {
                        var data = (List<VersionWithStructArray>)_versions;
                        data.Sort(VersionWithStructArray.Compare);
                    }
                    break;

                case Implementations.VersionWithTwoArrays:
                    {
                        var data = (List<VersionWithTwoArrays>)_versions;
                        data.Sort(VersionWithTwoArrays.Compare);
                    }
                    break;

                default:
                    throw new Exception("Unknown implementation " + Implementation);
            }
        }
    }
}
