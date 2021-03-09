using BenchmarkDotNet.Attributes;
using NuGet.Versioning;
using SemanticVersionBenchmarks.Implementations;
using SemanticVersionBenchmarks.Implementations.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using NuGetCopy = SemanticVersionBenchmarks.Implementations.NuGetCopy;
using NuGetCopy2 = SemanticVersionBenchmarks.Implementations.NuGetCopy2;
using NuGetCopy3 = SemanticVersionBenchmarks.Implementations.NuGetCopy3;
using NuGetCopy4 = SemanticVersionBenchmarks.Implementations.NuGetCopy4;
using NuGetCopy5 = SemanticVersionBenchmarks.Implementations.NuGetCopy5;
using NuGetCopy6 = SemanticVersionBenchmarks.Implementations.NuGetCopy6;
using NuGetCopy7 = SemanticVersionBenchmarks.Implementations.NuGetCopy7;

namespace SemanticVersionBenchmarks
{
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.NetCoreApp50)]
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net472)]
    [MarkdownExporterAttribute.GitHub]
    [MemoryDiagnoser]
    public class SortBenchmarks
    {
        private IList _versions;

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            proc.ProcessorAffinity = (IntPtr)0x1;

            var versions = await VersionData.GetVersionsAsync();

            static List<T> SetupInput<T>(List<string> strings, Func<string, T> parse, Comparison<T> comparer)
            {
                var input = new List<T>(strings.Count);
                for (int i = 0; i < strings.Count; i++)
                {
                    var version = parse(strings[i]);
                    input.Add(version);
                }

                input.Sort(comparer);

                return input;
            }

            static List<T> SetupNuGetInput<T>(List<string> strings, Func<NuGetVersion, T> parse, IComparer<T> comparer)
            {
                var input = new List<T>(strings.Count);
                for (int i = 0; i < strings.Count; i++)
                {
                    var nugetVersion = NuGetVersion.Parse(strings[i]);
                    var thisVersion = parse(nugetVersion);
                    input.Add(thisVersion);
                }

                input.Sort(comparer);

                return input;
            }

            switch (Implementation)
            {
                case Implementations.NuGet:
                    _versions = SetupNuGetInput<NuGetVersion>(versions, v => v, VersionComparer.Default);
                    break;

                case Implementations.NuGetCopy:
                    _versions =
                        SetupNuGetInput<NuGetCopy.NuGetVersion>(versions,
                            v => new NuGetCopy.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                            NuGetCopy.VersionComparer.Default);
                    break;

                case Implementations.NuGetCopy2:
                    _versions =
                        SetupNuGetInput<NuGetCopy2.NuGetVersion>(versions,
                            v => new NuGetCopy2.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                            NuGetCopy2.VersionComparer.Default);
                    break;

                case Implementations.NuGetCopy3:
                    _versions =
                        SetupNuGetInput<NuGetCopy3.NuGetVersion>(versions,
                            v => new NuGetCopy3.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                            NuGetCopy3.VersionComparer.Default);
                    break;

                case Implementations.NuGetCopy4:
                    _versions =
                        SetupNuGetInput<NuGetCopy4.NuGetVersion>(versions,
                            v => new NuGetCopy4.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                            NuGetCopy4.VersionComparer.Default);
                    break;

                case Implementations.NuGetCopy5:
                    _versions =
                        SetupNuGetInput<NuGetCopy5.NuGetVersion>(versions,
                            v => new NuGetCopy5.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                            NuGetCopy5.VersionComparer.Default);
                    break;

                case Implementations.NuGetCopy6:
                    _versions =
                        SetupNuGetInput<NuGetCopy6.NuGetVersion>(versions,
                            v => new NuGetCopy6.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                            NuGetCopy6.VersionComparer.Default);
                    break;

                case Implementations.NuGetCopy7:
                    _versions =
                        SetupNuGetInput<NuGetCopy7.NuGetVersion>(versions,
                            v => new NuGetCopy7.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                            NuGetCopy7.VersionComparer.Default);
                    break;

                case Implementations.VersionWithString:
                    _versions = SetupInput(versions, str => new VersionWithStringArray(str), VersionWithStringArray.Compare);
                    break;

                case Implementations.VersionWithClass:
                    _versions = SetupInput(versions, str => SubstringParser.Parse(str), VersionWithClassArray.Compare);
                    break;

                case Implementations.VersionWithStruct:
                    _versions = SetupInput(versions, str => new VersionWithStructArray(str), VersionWithStructArray.Compare);
                    break;

                case Implementations.VersionWithTwoArrays:
                    _versions = SetupInput(versions, str => new VersionWithTwoArrays(str), VersionWithTwoArrays.Compare);
                    break;

                default:
                    throw new Exception("Unkown implementation " + Implementation);
            }
        }

        public enum Implementations
        {
            NuGet,
            NuGetCopy,
            NuGetCopy2,
            NuGetCopy3,
            NuGetCopy4,
            NuGetCopy5,
            NuGetCopy6,
            NuGetCopy7,
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
            void Go<T>(Comparison<T> comparison)
            {
                var data = (List<T>)_versions;
                data.Sort(comparison);
            }

            switch (Implementation)
            {
                case Implementations.NuGet:
                    Go<NuGetVersion>(VersionComparer.Default.Compare);
                    break;

                case Implementations.NuGetCopy:
                    Go<SemanticVersionBenchmarks.Implementations.NuGetCopy.NuGetVersion>(SemanticVersionBenchmarks.Implementations.NuGetCopy.VersionComparer.Default.Compare);
                    break;

                case Implementations.NuGetCopy2:
                    Go<SemanticVersionBenchmarks.Implementations.NuGetCopy2.NuGetVersion>(SemanticVersionBenchmarks.Implementations.NuGetCopy2.VersionComparer.Default.Compare);
                    break;

                case Implementations.NuGetCopy3:
                    Go<SemanticVersionBenchmarks.Implementations.NuGetCopy3.NuGetVersion>(SemanticVersionBenchmarks.Implementations.NuGetCopy3.VersionComparer.Default.Compare);
                    break;

                case Implementations.NuGetCopy4:
                    Go<SemanticVersionBenchmarks.Implementations.NuGetCopy4.NuGetVersion>(SemanticVersionBenchmarks.Implementations.NuGetCopy4.VersionComparer.Default.Compare);
                    break;

                case Implementations.NuGetCopy5:
                    Go<SemanticVersionBenchmarks.Implementations.NuGetCopy5.NuGetVersion>(SemanticVersionBenchmarks.Implementations.NuGetCopy5.VersionComparer.Default.Compare);
                    break;

                case Implementations.NuGetCopy6:
                    Go<SemanticVersionBenchmarks.Implementations.NuGetCopy6.NuGetVersion>(SemanticVersionBenchmarks.Implementations.NuGetCopy6.VersionComparer.Default.Compare);
                    break;

                case Implementations.NuGetCopy7:
                    Go<SemanticVersionBenchmarks.Implementations.NuGetCopy7.NuGetVersion>(SemanticVersionBenchmarks.Implementations.NuGetCopy7.VersionComparer.Default.Compare);
                    break;

                case Implementations.VersionWithString:
                    Go<VersionWithStringArray>(VersionWithStringArray.Compare);
                    break;

                case Implementations.VersionWithClass:
                    Go<VersionWithClassArray>(VersionWithClassArray.Compare);
                    break;

                case Implementations.VersionWithStruct:
                    Go<VersionWithStructArray>(VersionWithStructArray.Compare);
                    break;

                case Implementations.VersionWithTwoArrays:
                    Go<VersionWithTwoArrays>(VersionWithTwoArrays.Compare);
                    break;

                default:
                    throw new Exception("Unknown implementation " + Implementation);
            }
        }
    }
}
