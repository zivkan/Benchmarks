﻿/* Copyright (c) .NET Foundation. All rights reserved.
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

*/

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticVersionBenchmarks.Implementations.NuGetCopy2
{
    public class NuGetVersion : SemanticVersion
    {
        private readonly string _originalString;

        public NuGetVersion(Version version, IEnumerable<string> releaseLabels, string metadata, string originalVersion)
            : base(version, releaseLabels, metadata)
        {
            _originalString = originalVersion;
        }

        public Version Version
        {
            get { return _version; }
        }

        public int Revision
        {
            get { return _version.Revision; }
        }

        public string OriginalVersion => _originalString;
    }

    public class SemanticVersion
    {
        internal readonly string[] _releaseLabels;
        internal readonly string _metadata;
        internal readonly Version _version;

        protected SemanticVersion(Version version, IEnumerable<string> releaseLabels, string metadata)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            _version = NormalizeVersionValue(version);
            _metadata = metadata;

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
            }
        }

        public int Major
        {
            get { return _version.Major; }
        }

        public int Minor
        {
            get { return _version.Minor; }
        }

        public int Patch
        {
            get { return _version.Build; }
        }

        public IEnumerable<string> ReleaseLabels
        {
            get { return _releaseLabels ?? EmptyReleaseLabels; }
        }

        public virtual bool IsPrerelease
        {
            get
            {
                if (_releaseLabels != null)
                {
                    for (int i = 0; i < _releaseLabels.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(_releaseLabels[i]))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public virtual string Metadata
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

        public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
        {
            return Equals(version1, version2);
        }

        public static bool operator !=(SemanticVersion version1, SemanticVersion version2)
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

        public bool Equals(SemanticVersion x, SemanticVersion y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(SemanticVersion version)
        {
            throw new NotImplementedException();
        }

        public int Compare(SemanticVersion x, SemanticVersion y)
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

            var legacyX = x as NuGetVersion;
            var legacyY = y as NuGetVersion;

            result = CompareLegacyVersion(legacyX, legacyY);
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

        private static int CompareLegacyVersion(NuGetVersion legacyX, NuGetVersion legacyY)
        {
            var result = 0;

            if (legacyX != null
                && legacyY != null)
            {
                result = legacyX.Version.CompareTo(legacyY.Version);
            }
            else if (legacyX != null
                     && legacyX.Version.Revision > 0)
            {
                result = 1;
            }
            else if (legacyY != null
                     && legacyY.Version.Revision > 0)
            {
                result = -1;
            }

            return result;
        }

        public static readonly IVersionComparer Default = new VersionComparer(VersionComparison.Default);

        private static int CompareReleaseLabels(string[] version1, string[] version2)
        {
            var result = 0;

            var count = Math.Max(version1.Length, version2.Length);

            for (var i = 0; i < count; i++)
            {
                var aExists = i < version1.Length;
                var bExists = i < version2.Length;

                if (!aExists && bExists)
                {
                    return -1;
                }

                if (aExists && !bExists)
                {
                    return 1;
                }

                result = CompareRelease(version1[i], version2[i]);

                if (result != 0)
                {
                    return result;
                }
            }

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

        private static string[] GetReleaseLabelsOrNull(SemanticVersion version)
        {
            string[] labels = null;

            if (version.IsPrerelease)
            {
                var enumerable = version.ReleaseLabels;
                labels = enumerable as string[];

                if (labels != null && enumerable != null)
                {
                    labels = enumerable.ToArray();
                }
            }

            return labels;
        }
    }

    public interface IVersionComparer : IEqualityComparer<SemanticVersion>, IComparer<SemanticVersion>
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
