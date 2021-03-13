# Semantic Version Benchmarks

These benchmarks are to test different implementations of version parsing and comparison. They are based on NuGet's version strings, which is more or less [Semantic Versioning 2](https://semver2.org) with the following differences:

1. There is a 4th numeric segment allowed, to be compatible with .NET's `System.Version` type (when no pre-release or metadata values are provided). This kind of 4 part version numbers are common throughout Microsoft products and has been used in Windows well before .NET was created.
2. Only the major version number is mandatory. Other version components are implicitly zero if not specified.
3. Pre-release label strings are case insensitive. SemVer2 doesn't specify whether they should be case sensitive or insensitive.

The project `SemanticVersionTests` ensures that my implementations sort the versions list the same way as `NuGet.Versioning`'s `NuGeVersion` and `VersionComparer.Default`, and the parsers have similar input validation. The project `SemanticVersionsBenchmarks` contains the implementations, as well as the benchmarks.

## Sort

* [See the results](SortBenchmarks.md)
* [Blog post testing different data structures for comparison performance](https://www.zivkan.com/blog/semver-sorting-performance-in-dotnet/)
* [Blog post investigating why my clean implementation of version comparison was much faster than NuGet's](https://www.zivkan.com/blog/nuget-version-performance-deep-dive/)
* [Pull request on NuGet.Client adding comparison performance improvements, from blog post learnings](https://github.com/NuGet/NuGet.Client/pull/3931)

## Parse

* [See the results](ParseBenchmarks.md)

## Version list used

This code, both the unit tests and benchmarks themselves, will enumerate nuget.org's catalog to determine the unique version strings used by all packages. This result is cached in a file, so subsequent runs are not slowed down.
