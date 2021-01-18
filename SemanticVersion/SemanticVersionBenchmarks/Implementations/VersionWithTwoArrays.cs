using System;
using System.Diagnostics;
using System.Linq;

namespace SemanticVersionBenchmarks.Implementations
{
    [DebuggerDisplay("{OriginalString}")]
    public class VersionWithTwoArrays
    {
        public string OriginalString { get; }
        public uint Major { get; }
        public uint Minor { get; }
        public uint Patch { get; }
        public uint? Legacy { get; }
        public string[]? Prerelease { get; }
        private uint?[]? _prereleaseInts;
        public string? Metadata { get; }

        public VersionWithTwoArrays(string originalString)
        {
            var prereleaseSeparatorIndex = originalString.IndexOf('-');
            var metadataSeparatorIndex = originalString.IndexOf('+');

            if (metadataSeparatorIndex > 0)
            {
                Metadata = originalString.Substring(metadataSeparatorIndex + 1);

                if (prereleaseSeparatorIndex > metadataSeparatorIndex)
                {
                    prereleaseSeparatorIndex = -1;
                }
            }
            else
            {
                Metadata = null;
                metadataSeparatorIndex = originalString.Length;
            }

            if (prereleaseSeparatorIndex > 0)
            {
                var preReleaseString = originalString.Substring(prereleaseSeparatorIndex + 1, metadataSeparatorIndex - prereleaseSeparatorIndex - 1);
                (Prerelease, _prereleaseInts) = ParsePreRelease(preReleaseString);
            }
            else
            {
                Prerelease = null;
                prereleaseSeparatorIndex = metadataSeparatorIndex;
            }

            var versionString = originalString.Substring(0, prereleaseSeparatorIndex);
            (Major, Minor, Patch, Legacy) = ParseVersion(versionString);

            OriginalString = originalString;
        }

        private (string[], uint?[]) ParsePreRelease(string preReleaseString)
        {
            var segments = preReleaseString.Count(c => c == '.') + 1;
            var stringSegments = new string[segments];
            var intSegments = new uint?[segments];
            uint intSegment;

            if (segments == 1)
            {
                stringSegments[0] = preReleaseString;
                if (uint.TryParse(preReleaseString, out intSegment))
                {
                    intSegments[0] = intSegment;
                }
                return (stringSegments, intSegments);
            }

            int lastIndex = -1;
            for (int i = 0; i < segments - 1; i++)
            {
                var nextIndex = preReleaseString.IndexOf('.', lastIndex + 1);
                if (nextIndex == -1)
                {
                    throw new Exception("bug");
                }

                var start = lastIndex + 1;
                var length = nextIndex - lastIndex - 1;
                stringSegments[i] = preReleaseString.Substring(start, length);
                if (uint.TryParse(stringSegments[i], out intSegment))
                {
                    intSegments[i] = intSegment;
                }
                lastIndex = nextIndex;
            }

            stringSegments[segments - 1] = preReleaseString.Substring(lastIndex + 1);
            if (uint.TryParse(stringSegments[segments - 1], out intSegment))
            {
                intSegments[segments - 1] = intSegment;
            }
            return (stringSegments, intSegments);
        }

        private (uint Major, uint Minor, uint Patch, uint? Legacy) ParseVersion(string versionString)
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

        public static int Compare(VersionWithTwoArrays x, VersionWithTwoArrays y)
        {
            var cmp = x.Major.CompareTo(y.Major);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = x.Minor.CompareTo(y.Minor);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = x.Patch.CompareTo(y.Patch);
            if (cmp != 0)
            {
                return cmp;
            }

            var xl = x.Legacy ?? 0;
            var yl = y.Legacy ?? 0;
            cmp = xl.CompareTo(yl);
            if (cmp != 0)
            {
                return cmp;
            }

            cmp = ComparePreRelease(x.Prerelease, x._prereleaseInts , y.Prerelease, y._prereleaseInts);
            return cmp;
        }

        private static int ComparePreRelease(string[]? xStringSegments, uint?[]? xIntSegments, string[]? yStringSegments, uint?[]? yIntSegments)
        {
            if (xStringSegments  == null || yStringSegments == null)
            {
                if (xStringSegments != null)
                {
                    return -1;
                }
                else if (yStringSegments != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            if (xIntSegments == null || yIntSegments == null)
            {
                throw new Exception("Bug");
            }

            var maxSegments = Math.Min(xStringSegments.Length, yStringSegments.Length);
            for (int i = 0; i < maxSegments; i++)
            {
                var xs = xIntSegments[i];
                var ys = yIntSegments[i];

                if (xs.HasValue || ys.HasValue)
                {
                    if (!xs.HasValue)
                    {
                        return 1;
                    }

                    if (!ys.HasValue)
                    {
                        return -1;
                    }

                    var cmp = xs.Value.CompareTo(ys.Value);
                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }
                else
                {
                    var cmp = StringComparer.OrdinalIgnoreCase.Compare(xStringSegments[i], yStringSegments[i]);
                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }
            }

            var lengthCmp = xStringSegments.Length.CompareTo(yStringSegments.Length);
            return lengthCmp;
        }
    }
}
