using FluentAssertions;
using NuGet.Versioning;
using SemanticVersionBenchmarks;
using SemanticVersionBenchmarks.Implementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SemanticVersionTests
{
    public class VersionSortTests
    {
        List<string> input;
        List<string> expected;

        public VersionSortTests()
        {
            ClassSetupAsync().GetAwaiter().GetResult();
        }

        private async Task ClassSetupAsync()
        {
            var versions = await VersionData.GetVersionsAsync(false);

            var rand = new Random(1);
            for (int i = versions.Count-1; i > 0; i--)
            {
                var index = rand.Next(0, i + 1);
                if (index != i)
                {
                    var temp = versions[i];
                    versions[i] = versions[index];
                    versions[index] = temp;
                }
            }

            input = versions;

            var nugetVersions = new List<NuGetVersion>(input.Count);
            for (int i = 0; i < versions.Count; i++)
            {
                var stringVersion = input[i];
                var nugetVersion = NuGetVersion.Parse(stringVersion);
                nugetVersions.Add(nugetVersion);
            }

            nugetVersions.Sort(VersionComparer.Default);

            expected = new List<string>(nugetVersions.Count);
            for (int i = 0; i < nugetVersions.Count; i++)
            {
                expected.Add(nugetVersions[i].OriginalVersion);
            }
        }

        [Fact]
        public void VersionWithStringArraySort()
        {
            var versions = new List<VersionWithStringArray>(input.Count);
            for (int i =0; i < input.Count; i++)
            {
                versions.Add(new VersionWithStringArray(input[i]));
            }

            versions.Sort(VersionWithStringArray.Compare);

            var result = new List<string>();
            for (int i = 0; i < versions.Count; i++)
            {
                result.Add(versions[i].OriginalString);
            }

            result.Should().Equal(expected);
        }

        [Fact]
        public void VersionWithClassArraySort()
        {
            var versions = new List<VersionWithClassArray>(input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                versions.Add(new VersionWithClassArray(input[i]));
            }

            versions.Sort(VersionWithClassArray.Compare);

            var result = new List<string>();
            for (int i = 0; i < versions.Count; i++)
            {
                result.Add(versions[i].OriginalString);
            }

            result.Should().Equal(expected);
        }

        [Fact]
        public void VersionWithStructArraySort()
        {
            var versions = new List<VersionWithStructArray>(input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                versions.Add(new VersionWithStructArray(input[i]));
            }

            versions.Sort(VersionWithStructArray.Compare);

            var result = new List<string>();
            for (int i = 0; i < versions.Count; i++)
            {
                result.Add(versions[i].OriginalString);
            }

            result.Should().Equal(expected);
        }

        [Fact]
        public void VersionWithTwoArraySort()
        {
            var versions = new List<VersionWithTwoArrays>(input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                versions.Add(new VersionWithTwoArrays(input[i]));
            }

            versions.Sort(VersionWithTwoArrays.Compare);

            var result = new List<string>();
            for (int i = 0; i < versions.Count; i++)
            {
                result.Add(versions[i].OriginalString);
            }

            result.Should().Equal(expected);
        }

        [Fact]
        public void NuGetCopy()
        {
            RunNuGetCopy(
                v => new SemanticVersionBenchmarks.Implementations.NuGetCopy.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                SemanticVersionBenchmarks.Implementations.NuGetCopy.VersionComparer.Default.Compare,
                v => v.OriginalVersion);
        }

        [Fact]
        public void NuGetCopy2()
        {
            RunNuGetCopy(
                v => new SemanticVersionBenchmarks.Implementations.NuGetCopy2.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                SemanticVersionBenchmarks.Implementations.NuGetCopy2.VersionComparer.Default.Compare,
                v => v.OriginalVersion);
        }

        [Fact]
        public void NuGetCopy3()
        {
            RunNuGetCopy(
                v => new SemanticVersionBenchmarks.Implementations.NuGetCopy3.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                SemanticVersionBenchmarks.Implementations.NuGetCopy3.VersionComparer.Default.Compare,
                v => v.OriginalVersion);
        }

        [Fact]
        public void NuGetCopy4()
        {
            RunNuGetCopy(
                v => new SemanticVersionBenchmarks.Implementations.NuGetCopy4.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                SemanticVersionBenchmarks.Implementations.NuGetCopy4.VersionComparer.Default.Compare,
                v => v.OriginalVersion);
        }

        [Fact]
        public void NuGetCopy5()
        {
            RunNuGetCopy(
                v => new SemanticVersionBenchmarks.Implementations.NuGetCopy5.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                SemanticVersionBenchmarks.Implementations.NuGetCopy5.VersionComparer.Default.Compare,
                v => v.OriginalVersion);
        }

        [Fact]
        public void NuGetCopy6()
        {
            RunNuGetCopy(
                v => new SemanticVersionBenchmarks.Implementations.NuGetCopy6.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                SemanticVersionBenchmarks.Implementations.NuGetCopy6.VersionComparer.Default.Compare,
                v => v.OriginalVersion);
        }

        [Fact]
        public void NuGetCopy7()
        {
            RunNuGetCopy(
                v => new SemanticVersionBenchmarks.Implementations.NuGetCopy7.NuGetVersion(v.Version, v.ReleaseLabels, v.Metadata, v.OriginalVersion),
                SemanticVersionBenchmarks.Implementations.NuGetCopy7.VersionComparer.Default.Compare,
                v => v.OriginalVersion);
        }

        private void RunNuGetCopy<T>(Func<NuGetVersion, T> parse, Comparison<T> comparer, Func<T, string> originalString)
        {
            var versions = new List<T>(input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                var nugetVersion = NuGetVersion.Parse(input[i]);
                versions.Add(parse(nugetVersion));
            }

            versions.Sort(comparer);

            var result = new List<string>();
            for (int i = 0; i < versions.Count; i++)
            {
                result.Add(originalString(versions[i]));
            }

            result.Should().Equal(expected);
        }
    }
}
