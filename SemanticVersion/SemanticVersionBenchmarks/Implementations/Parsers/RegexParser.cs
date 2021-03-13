using System;
using System.Text.RegularExpressions;

namespace SemanticVersionBenchmarks.Implementations.Parsers
{
    public static class RegexParser
    {
        // Copied from https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string
        // Modified to use .NET RegEx syntax, and to be compatible with NuGetVersion, which is more permissive than SemVer2.
        private static Regex _regex = new Regex(@"^(?<major>0|[1-9]\d*)(\.(?<minor>\d+)(\.(?<patch>\d+)(\.(?<revision>\d+))?)?)?(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$", RegexOptions.CultureInvariant|RegexOptions.Singleline|RegexOptions.Compiled);

        public static VersionWithClassArray Parse(string input)
        {
            var match = _regex.Match(input);
            if (!match.Success)
            {
                throw new FormatException("Did not match regex");
            }

            var majorString = match.Groups["major"].Value;
            var major = uint.Parse(majorString);

            var minorGroup = match.Groups["minor"];
            var minor = minorGroup.Success
                ? uint.Parse(minorGroup.Value)
                : 0;

            var patchGroup = match.Groups["patch"];
            var patch = patchGroup.Success
                ? uint.Parse(patchGroup.Value)
                : 0;

            var revisionGroup = match.Groups["revision"];
            var revision = revisionGroup.Success
                ? uint.Parse(revisionGroup.Value)
                : (uint?)null;

            var prereleaseGroup = match.Groups["prerelease"];
            VersionWithClassArray.PrereleaseSegment[]? prerelease = null;
            if (prereleaseGroup.Success)
            {
                string[] split = prereleaseGroup.Value.Split('.');
                prerelease = new VersionWithClassArray.PrereleaseSegment[split.Length];
                for (int i = 0; i < split.Length; i++)
                {
                    prerelease[i] = new VersionWithClassArray.PrereleaseSegment(split[i]);
                }
            }

            var metadataGroup = match.Groups["buildmetadata"];
            string? metadata = metadataGroup.Success
                ? metadataGroup.Value
                : null;

            return new VersionWithClassArray(input, major, minor, patch, revision, prerelease, metadata);
        }
    }
}
