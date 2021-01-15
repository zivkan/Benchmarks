using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticVersionBenchmarks.Implementations
{
    [DebuggerDisplay("{OriginalString}")]
    public class VersionWithStructArray
    {
        public string OriginalString { get; }
        public uint Major { get; }
        public uint Minor { get; }
        public uint Patch { get; }
        public uint? Legacy { get; }
        public PrereleaseSegment[]? Prerelease { get; }
        public string? Metadata { get; }

        public VersionWithStructArray(string originalString)
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
                Prerelease = ParsePreRelease(preReleaseString);
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

        private PrereleaseSegment[] ParsePreRelease(string preReleaseString)
        {
            var segments = new PrereleaseSegment[preReleaseString.Count(c => c == '.') + 1];

            if (segments.Length == 1)
            {
                segments[0] = new PrereleaseSegment(preReleaseString);
                return segments;
            }

            int lastIndex = -1;
            for (int i = 0; i < segments.Length - 1; i++)
            {
                var nextIndex = preReleaseString.IndexOf('.', lastIndex + 1);
                if (nextIndex == -1)
                {
                    throw new Exception("bug");
                }

                var start = lastIndex + 1;
                var length = nextIndex - lastIndex - 1;
                segments[i] = new PrereleaseSegment(preReleaseString.Substring(start, length));
                lastIndex = nextIndex;
            }

            segments[segments.Length - 1] = new PrereleaseSegment(preReleaseString.Substring(lastIndex + 1));
            return segments;
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

        public static int Compare(VersionWithStructArray x, VersionWithStructArray y)
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

            cmp = ComparePreRelease(x.Prerelease, y.Prerelease);
            return cmp;
        }

        private static int ComparePreRelease(PrereleaseSegment[]? x, PrereleaseSegment[]? y)
        {
            if (x == null || y == null)
            {
                if (x != null)
                {
                    return -1;
                }
                else if (y != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            var maxSegments = Math.Min(x.Length, y.Length);
            for (int i = 0; i < maxSegments; i++)
            {
                var xs = x[i];
                var ys = y[i];

                if (xs.Int.HasValue || ys.Int.HasValue)
                {
                    if (!xs.Int.HasValue)
                    {
                        return 1;
                    }

                    if (!ys.Int.HasValue)
                    {
                        return -1;
                    }

                    var cmp = xs.Int.Value.CompareTo(ys.Int.Value);
                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }
                else
                {
                    var cmp = StringComparer.OrdinalIgnoreCase.Compare(xs.Value, ys.Value);
                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }
            }

            var lengthCmp = x.Length.CompareTo(y.Length);
            return lengthCmp;
        }

        public struct PrereleaseSegment
        {
            public string Value { get; }
            public uint? Int { get; }

            public PrereleaseSegment(string value)
            {
                Value = value;

                Int = uint.TryParse(value, out uint result)
                    ? (uint?)result
                    : null;
            }
        }
    }
}
