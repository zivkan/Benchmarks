using System;
using System.Diagnostics;
using System.Linq;

namespace SemanticVersionBenchmarks.Implementations
{
    [DebuggerDisplay("{OriginalString}")]
    public class VersionWithClassArray
    {
        public string OriginalString { get; }
        public uint Major { get; }
        public uint Minor { get; }
        public uint Patch { get; }
        public uint? Legacy { get; }
        public PrereleaseSegment[]? Prerelease { get; }
        public string? Metadata { get; }

        public VersionWithClassArray(string originalString, uint major, uint minor, uint patch, uint? legacy, PrereleaseSegment[]? prerelease, string? metadata)
        {
            OriginalString = originalString;
            Major = major;
            Minor = minor;
            Patch = patch;
            Legacy = legacy;
            Prerelease = prerelease;
            Metadata = metadata;
        }

        public static int Compare(VersionWithClassArray x, VersionWithClassArray y)
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

        public class PrereleaseSegment
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
