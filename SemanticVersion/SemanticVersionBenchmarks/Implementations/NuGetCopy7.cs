/* Copyright (c) .NET Foundation. All rights reserved.
   Licensed under the Apache License, Version 2.0. See https://github.com/NuGet/NuGet.Client/blob/27e64ba7b38abda73ac20173eb6ffaf8b1c2d3d7/LICENSE.txt for license information.

The contents of this file has been copied from https://github.com/NuGet/NuGet.Client
It has the following modifications:
* All the files were concatenated into a single file to be easier to modify for this project.
* The TypeConverterAttribute was removed from SemanticVersion.
* Three exception messages were changed to avoid needing `Strings.resx`.

NuGetCopy2 has the following modifications:
* Comments removed to make the file shorter.
* Methods and classes not used by the sorting benchmark has been removed.
* Partial classes were merged into a single non-partial class.
* Changed SemanticVersion's GetHashCode and Equals(object) implementations to throw NotImplementedException, so the code they use can be deleted, while being confident that it's not used.

NuGetCopy3 has the following modifications:
* Fixed bug in GetReleaseLabelsOrNull where `labels != null` should be `labels == null`

NuGetCopy4 has the following modifications:
* NuGetVersion and SemanticVersion stores Major, Minor, Patch and Revision ints directly, rather than via System.Version.
* CompareLegacyVersion was removed, and Compare compres the revision component directly.

NuGetCopy5 has the following modifications:
* CompareReleaseLabels uses Math.Min, rather than Math.Max, for the iteration count, and compares the two inputs lengths at the end of the loop, rather than every iteration.

NuGetCopy6 has the following modifications:
* SemanticVersioning's constructor validates that none of the prerelease label segments are null or empty.
* IsPrelease changed to auto-properties set in the constructor, to reduce per-invocation logic.
* _releaseLabels set to EmptyReleaseLabels in constructor, to remove logic in property getter;

NuGetCopy7 has the following modifications:
* SemanticVersion was merged into NuGetVersion, which was also marked as sealed.
* IsPrerelease and ReleaseLabels had the virtual keyword removed.
* All methods that used SemanticVersion as parameters were changed to use NuGetVersion.
* The compare method no longer does typecasting and null checking since the instances are always NuGetVersion now.

*/

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticVersionBenchmarks.Implementations.NuGetCopy7
{
    public sealed class NuGetVersion
    {
        private readonly string _originalString;

        public NuGetVersion(Version version, IEnumerable<string> releaseLabels, string metadata, string originalVersion)
            : this(version, releaseLabels, metadata)
        {
            _originalString = originalVersion;
            Version = NormalizeVersionValue(version);
            Revision = Version.Revision;
        }

        public Version Version { get; }

        public int Revision { get; }

        public string OriginalVersion => _originalString;

        internal readonly string[] _releaseLabels;
        internal readonly string _metadata;

        internal NuGetVersion(Version version, IEnumerable<string> releaseLabels, string metadata)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            var normalizedVersion = NormalizeVersionValue(version);
            Major = normalizedVersion.Major;
            Minor = normalizedVersion.Minor;
            Patch = normalizedVersion.Build;

            _metadata = metadata;

            IsPrerelease = true;
            _releaseLabels = EmptyReleaseLabels;

            if (releaseLabels != null)
            {
                var asArray = releaseLabels as string[];

                if (asArray != null)
                {
                    _releaseLabels = asArray;
                }
                else
                {
                    _releaseLabels = releaseLabels.ToArray();
                }

                if (_releaseLabels.Length < 1)
                {
                    _releaseLabels = null;
                }
                else
                {
                    if (_releaseLabels.Any(l => string.IsNullOrEmpty(l)))
                    {
                        throw new ArgumentException(message: "release labels must not have null or empty item", paramName: nameof(releaseLabels));
                    }
                }
            }
        }

        public int Major { get; }

        public int Minor { get; }

        public int Patch { get; }

        public IEnumerable<string> ReleaseLabels
        {
            get { return _releaseLabels; }
        }

        public bool IsPrerelease { get; }

        public string Metadata
        {
            get { return _metadata; }
        }

        internal static readonly string[] EmptyReleaseLabels = Array.Empty<string>();

        internal static Version NormalizeVersionValue(Version version)
        {
            var normalized = version;

            if (version.Build < 0
                || version.Revision < 0)
            {
                normalized = new Version(
                    version.Major,
                    version.Minor,
                    Math.Max(version.Build, 0),
                    Math.Max(version.Revision, 0));
            }

            return normalized;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(NuGetVersion version1, NuGetVersion version2)
        {
            return Equals(version1, version2);
        }

        public static bool operator !=(NuGetVersion version1, NuGetVersion version2)
        {
            return !Equals(version1, version2);
        }
    }

    public sealed class VersionComparer : IVersionComparer
    {
        private readonly VersionComparison _mode;

        public VersionComparer(VersionComparison versionComparison)
        {
            _mode = versionComparison;
        }

        public bool Equals(NuGetVersion x, NuGetVersion y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(NuGetVersion version)
        {
            throw new NotImplementedException();
        }

        public int Compare(NuGetVersion x, NuGetVersion y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (ReferenceEquals(y, null))
            {
                return 1;
            }

            if (ReferenceEquals(x, null))
            {
                return -1;
            }

            var result = x.Major.CompareTo(y.Major);
            if (result != 0)
            {
                return result;
            }

            result = x.Minor.CompareTo(y.Minor);
            if (result != 0)
            {
                return result;
            }

            result = x.Patch.CompareTo(y.Patch);
            if (result != 0)
            {
                return result;
            }

            result = x.Revision.CompareTo(y.Revision);
            if (result != 0)
            {
                return result;
            }

            if (_mode != VersionComparison.Version)
            {
                var xLabels = GetReleaseLabelsOrNull(x);
                var yLabels = GetReleaseLabelsOrNull(y);

                if (xLabels != null
                    && yLabels == null)
                {
                    return -1;
                }

                if (xLabels == null
                    && yLabels != null)
                {
                    return 1;
                }

                if (xLabels != null
                    && yLabels != null)
                {
                    result = CompareReleaseLabels(xLabels, yLabels);
                    if (result != 0)
                    {
                        return result;
                    }
                }

                if (_mode == VersionComparison.VersionReleaseMetadata)
                {
                    result = StringComparer.OrdinalIgnoreCase.Compare(x.Metadata ?? string.Empty, y.Metadata ?? string.Empty);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }

            return 0;
        }

        public static readonly IVersionComparer Default = new VersionComparer(VersionComparison.Default);

        private static int CompareReleaseLabels(string[] version1, string[] version2)
        {
            var count = Math.Min(version1.Length, version2.Length);

            int result;
            for (var i = 0; i < count; i++)
            {
                result = CompareRelease(version1[i], version2[i]);

                if (result != 0)
                {
                    return result;
                }
            }

            result = version1.Length.CompareTo(version2.Length);
            return result;
        }

        private static int CompareRelease(string version1, string version2)
        {
            var version1Num = 0;
            var version2Num = 0;
            var result = 0;

            var v1IsNumeric = int.TryParse(version1, out version1Num);
            var v2IsNumeric = int.TryParse(version2, out version2Num);

            if (v1IsNumeric && v2IsNumeric)
            {
                result = version1Num.CompareTo(version2Num);
            }
            else if (v1IsNumeric || v2IsNumeric)
            {
                if (v1IsNumeric)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
            else
            {
                result = StringComparer.OrdinalIgnoreCase.Compare(version1, version2);
            }

            return result;
        }

        private static string[] GetReleaseLabelsOrNull(NuGetVersion version)
        {
            string[] labels = null;

            if (version.IsPrerelease)
            {
                var enumerable = version.ReleaseLabels;
                labels = enumerable as string[];

                if (labels == null && enumerable != null)
                {
                    labels = enumerable.ToArray();
                }
            }

            return labels;
        }
    }

    public interface IVersionComparer : IEqualityComparer<NuGetVersion>, IComparer<NuGetVersion>
    {
    }

    public enum VersionComparison
    {
        Default = 0,
        Version = 1,
        VersionRelease = 2,
        VersionReleaseMetadata = 3
    }
}
