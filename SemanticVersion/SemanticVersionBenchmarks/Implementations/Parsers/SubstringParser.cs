using System;
using System.Linq;

namespace SemanticVersionBenchmarks.Implementations.Parsers
{
    public static class SubstringParser
    {
        public static VersionWithClassArray Parse(string originalString)
        {
            var prereleaseSeparatorIndex = originalString.IndexOf('-');
            var metadataSeparatorIndex = originalString.IndexOf('+');

            string? metadata = null;
            if (metadataSeparatorIndex > 0)
            {
                metadata = originalString.Substring(metadataSeparatorIndex + 1);
                ParserShared.ValidateMetadata(metadata);

                if (prereleaseSeparatorIndex > metadataSeparatorIndex)
                {
                    prereleaseSeparatorIndex = -1;
                }
            }
            else
            {
                metadataSeparatorIndex = originalString.Length;
            }

            VersionWithClassArray.PrereleaseSegment[]? prerelease = null;
            if (prereleaseSeparatorIndex > 0)
            {
                var preReleaseString = originalString.Substring(prereleaseSeparatorIndex + 1, metadataSeparatorIndex - prereleaseSeparatorIndex - 1);
                prerelease = ParsePreRelease(preReleaseString);
            }
            else
            {
                prereleaseSeparatorIndex = metadataSeparatorIndex;
            }

            var versionString = originalString.Substring(0, prereleaseSeparatorIndex);
            var (major, minor, patch, legacy) = ParseVersion(versionString);

            return new VersionWithClassArray(originalString, major, minor, patch, legacy, prerelease, metadata);
        }

        private static VersionWithClassArray.PrereleaseSegment[] ParsePreRelease(string preReleaseString)
        {
            var segments = new VersionWithClassArray.PrereleaseSegment[preReleaseString.Count(c => c == '.') + 1];

            if (segments.Length == 1)
            {
                IsValid(preReleaseString);
                segments[0] = new VersionWithClassArray.PrereleaseSegment(preReleaseString);
                return segments;
            }

            int lastIndex = -1;
            string segment;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                var nextIndex = preReleaseString.IndexOf('.', lastIndex + 1);
                if (nextIndex == -1)
                {
                    throw new Exception("bug");
                }

                var start = lastIndex + 1;
                var length = nextIndex - lastIndex - 1;
                segment = preReleaseString.Substring(start, length);
                IsValid(segment);
                segments[i] = new VersionWithClassArray.PrereleaseSegment(segment);
                IsValid(segments[i].Value);
                lastIndex = nextIndex;
            }

            segment = preReleaseString.Substring(lastIndex + 1);
            IsValid(segment);
            segments[segments.Length - 1] = new VersionWithClassArray.PrereleaseSegment(segment);
            return segments;

            static bool IsValid(string segment)
            {
                if (segment.Length == 0)
                {
                    throw new ArgumentException();
                }

                for (int i = 0; i < segment.Length; i++)
                {
                    char c = segment[i];
                    if (!((c >= '0' && c <= '9')
                        || (c >= 'a' && c <= 'z')
                        || (c >= 'A' && c <= 'Z')
                        || c == '.' || c == '-'))
                    {
                        throw new ArgumentException();
                    }
                }

                return true;
            }
        }

        private static (uint Major, uint Minor, uint Patch, uint? Legacy) ParseVersion(string versionString)
        {
            var firstDotIndex = versionString.IndexOf('.');
            uint major;
            if (firstDotIndex < 0)
            {
                major = uint.Parse(versionString);
                return (major, 0, 0, null);
            }

            var firstSegment = versionString.Substring(0, firstDotIndex);
            major = uint.Parse(firstSegment);

            var secondDotIndex = versionString.IndexOf('.', firstDotIndex + 1);
            string secondSegment;
            uint minor;
            if (secondDotIndex < 0)
            {
                secondSegment = versionString.Substring(firstDotIndex + 1);
                minor = uint.Parse(secondSegment);
                return (major, minor, 0, null);
            }

            secondSegment = versionString.Substring(firstDotIndex + 1, secondDotIndex - firstDotIndex - 1);
            minor = uint.Parse(secondSegment);

            var thirdDotIndex = versionString.IndexOf('.', secondDotIndex + 1);
            string thirdSegment;
            uint patch;
            uint? legacy;
            if (thirdDotIndex < 0)
            {
                thirdSegment = versionString.Substring(secondDotIndex + 1);
                patch = uint.Parse(thirdSegment);
                legacy = null;
            }
            else
            {
                thirdSegment = versionString.Substring(secondDotIndex + 1, thirdDotIndex - secondDotIndex - 1);
                patch = uint.Parse(thirdSegment);
                var legacySegment = versionString.Substring(thirdDotIndex + 1);
                legacy = uint.Parse(legacySegment);
            }

            return (major, minor, patch, legacy);
        }
    }
}
