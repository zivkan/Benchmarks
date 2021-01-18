# Semantic Version Benchmarks

These benchmarks are to test different implementations of version parsing and comparison. They are based on NuGet's version strings, which is more or less [Semantic Versioning 2](https://semver2.org) with the following differences:

1. There is a 4th numeric segment allowed, to be compatible with .NET's `System.Version` type (when no pre-release or metadata values are provided). This kind of 4 part version numbers are common throughout Microsoft products and has been used in Windows well before .NET was created.
2. Pre-release label strings are case insensitive, rather than case sensitive.

Additionally, I'm not sure if this is consistent or diverges from the semver2 spec, but NuGet's version sorting sorts pre-release label segments that are all numeric before pre-release label segments that are alphanumeric. For example, a lexicographic sort would put `1.0.0-11a` before `1.0.0-12`, but NuGet sorts them the other way around.

The project `SemanticVersionTests` ensures that my implementations sort the versions list the same way as `NuGet.Versioning`'s `NuGeVersion` and `VersionComparer.Default`. The project `SemanticVersionsBenchmarks` contains the implementations, as well as the benchmarks.

## Sort

[See the results](SortBenchmarks.md)

## Parse

Not yet implemented.

## Version list used

This code, both the unit tests and benchmarks themselves, will enumerate nuget.org's catalog to determine the unique version strings used by all packages. This result is cached in a file, so subsequent runs are not slowed down.
