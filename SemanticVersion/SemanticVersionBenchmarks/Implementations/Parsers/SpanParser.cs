using System;

namespace SemanticVersionBenchmarks.Implementations.Parsers
{
    public static class SpanParser
    {
        public static VersionWithClassArray Parse(string originalString)
        {
            var prereleaseSeparatorIndex = originalString.IndexOf('-');
            var metadataSeparatorIndex = originalString.IndexOf('+');

            string? metadata = null;
            if (metadataSeparatorIndex > 0)
            {
                metadata = originalString.Substring(metadataSeparatorIndex + 1);
                ValidateMetadata(metadata);

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
                var preReleaseString = originalString.AsSpan(prereleaseSeparatorIndex + 1, metadataSeparatorIndex - prereleaseSeparatorIndex - 1);
                prerelease = ParsePreRelease(preReleaseString);
            }
            else
            {
                prereleaseSeparatorIndex = metadataSeparatorIndex;
            }

            var versionString = originalString.AsSpan(0, prereleaseSeparatorIndex);
            var (major, minor, patch, legacy) = ParseVersion(versionString);

            return new VersionWithClassArray(originalString, major, minor, patch, legacy, prerelease, metadata);
        }

        private static VersionWithClassArray.PrereleaseSegment[] ParsePreRelease(ReadOnlySpan<char> preReleaseString)
        {
            var segmentCount = 1;
            for (int i = 0; i < preReleaseString.Length; i++)
            {
                if (preReleaseString[i] == '.')
                {
                    segmentCount++;
                }
            }

            var segments = new VersionWithClassArray.PrereleaseSegment[segmentCount];

            if (segments.Length == 1)
            {
                IsValid(preReleaseString);
                segments[0] = new VersionWithClassArray.PrereleaseSegment(preReleaseString.ToString());
                return segments;
            }

            for (int i = 0; i < segments.Length - 1; i++)
            {
                var nextIndex = preReleaseString.IndexOf('.');
                if (nextIndex == -1)
                {
                    throw new Exception("bug");
                }

                var slice = preReleaseString.Slice(0, nextIndex);
                IsValid(slice);
                segments[i] = new VersionWithClassArray.PrereleaseSegment(slice.ToString());
                preReleaseString = preReleaseString.Slice(nextIndex + 1);
            }

            if (preReleaseString.Length == 0)
            {
                throw new FormatException();
            }
            IsValid(preReleaseString);
            segments[segments.Length - 1] = new VersionWithClassArray.PrereleaseSegment(preReleaseString.ToString());
            return segments;

            static bool IsValid(ReadOnlySpan<char> segment)
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
                        || c =='.' || c== '-'))
                    {
                        throw new ArgumentException();
                    }
                }

                return true;
            }
        }

        private static (uint Major, uint Minor, uint Patch, uint? Legacy) ParseVersion(ReadOnlySpan<char> versionString)
        {
            var dotIndex = versionString.IndexOf('.');
            uint major;
            if (dotIndex < 0)
            {
#if !NETFRAMEWORK
                major = uint.Parse(versionString);
#else
                major = Parse(versionString);
#endif
                return (major, 0, 0, null);
            }

#if !NETFRAMEWORK
            major = uint.Parse(versionString.Slice(0, dotIndex));
#else
            major = Parse(versionString.Slice(0, dotIndex));
#endif

            versionString = versionString.Slice(dotIndex + 1);
            dotIndex = versionString.IndexOf('.');
            uint minor;
            if (dotIndex < 0)
            {
#if !NETFRAMEWORK
                minor = uint.Parse(versionString);
#else
                minor = Parse(versionString);
#endif
                return (major, minor, 0, null);
            }

#if !NETFRAMEWORK
            minor = uint.Parse(versionString.Slice(0, dotIndex));
#else
            minor = Parse(versionString.Slice(0, dotIndex));
#endif

            versionString = versionString.Slice(dotIndex + 1);
            dotIndex = versionString.IndexOf('.');
            uint patch;
            uint? legacy;
            if (dotIndex < 0)
            {
#if !NETFRAMEWORK
                patch = uint.Parse(versionString);
#else
                patch = Parse(versionString);
#endif
                legacy = null;
            }
            else
            {
#if !NETFRAMEWORK
                patch = uint.Parse(versionString.Slice(0, dotIndex));
                legacy = uint.Parse(versionString.Slice(dotIndex + 1));
#else
                patch = Parse(versionString.Slice(0, dotIndex));
                legacy = Parse(versionString.Slice(dotIndex + 1));
#endif
            }

            return (major, minor, patch, legacy);
        }

        private static void ValidateMetadata(string metadata)
        {
            if (metadata.Length == 0)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < metadata.Length; i++)
            {
                char c = metadata[i];
                if (!((c >= '0' && c <= '9')
                    || (c >= 'a' && c <= 'z')
                    || (c >= 'A' && c <= 'Z')
                    || c == '.' || c == '-'))
                {
                    throw new ArgumentException();
                }

                if (c == '.' && (i == 0 || metadata[i - 1] == '.'))
                {
                    throw new ArgumentException();
                }
            }

            if (metadata[metadata.Length - 1] == '.')
            {
                throw new ArgumentException();
            }
        }

#if NETFRAMEWORK
        private static uint Parse(ReadOnlySpan<char> input)
        {
            checked
            {
                uint result = 0;
                foreach(var c in input)
                {
                    if (c < '0' || c > '9')
                    {
                        throw new FormatException();
                    }

                    result = result * 10 + (uint)(c - '0');
                }

                return result;
            }
        }
#endif
    }
}
