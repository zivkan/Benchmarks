using FluentAssertions;
using NuGet.Versioning;
using SemanticVersionBenchmarks;
using SemanticVersionBenchmarks.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
    }
}
