# Write Performance

In mid-late 2020, [NuGet started extracting files using Memory Mapped IO](https://github.com/NuGet/NuGet.Client/pull/3524), rather than the `FileStream` based APIs. The corresponded to NuGet 5.8, Visual Studio 2020 16.8 and .NET 5.0 SDK 5.0.100.

About a year later, [marcin-krystianc](https://github.com/marcin-krystianc) [created a pull request reverting the change](https://github.com/NuGet/NuGet.Client/pull/4186) with benchmarks showing that mmap was slower on his tests than stream based IO. However, testing NuGet restore on my machine with mmap and stream IO, I found mmap to be faster.

This benchmark is designed to help investigate what is faster and when, and therefore help decide which APIs NuGet should use when extracting packages.

## Results

I will have an upcoming blog post. While my other benchmarks in this repo have result files that are updated automatically when I run the benchmark, making it easy for me to commit the results, this benchmark differs wildly between machines and operating systems.

## Benchmarks

There are two benchmark classes, testing different things.

### Write Only

This benchmark tests only writing files from a pre-created buffer. Its goal is to measure which API is fastest at writing files, without any other overhead.

### Nuget Package Extraction

This benchmark pre-downloads all the Nuget packages that Orchard Core uses, then each iteration of the benchmark attempts to extract them as quickly as possible.Its goal is to see what differences in write performance exist compared to the write only benchmark.
