``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.200
  [Host]        : .NET Core 5.0.3 (CoreCLR 5.0.321.7212, CoreFX 5.0.321.7212), X64 RyuJIT
  .NET 4.7.2    : .NET Framework 4.8 (4.8.4300.0), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.12 (CoreCLR 4.700.21.6504, CoreFX 4.700.21.6905), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.3 (CoreCLR 5.0.321.7212, CoreFX 5.0.321.7212), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  

```
| Method |           Job |       Runtime |       Implementation |     Mean |   Error |  StdDev |   Median |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|------- |-------------- |-------------- |--------------------- |---------:|--------:|--------:|---------:|------------:|------:|------:|-----------:|
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |                **NuGet** | **709.4 ms** | **1.17 ms** | **1.04 ms** | **709.6 ms** | **111000.0000** |     **-** |     **-** | **87636256 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |                NuGet | 435.7 ms | 3.89 ms | 3.64 ms | 435.4 ms |  20000.0000 |     - |     - | 87373056 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |                NuGet | 470.9 ms | 2.15 ms | 1.80 ms | 471.4 ms |  20000.0000 |     - |     - | 87373592 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |            **NuGetCopy** | **704.6 ms** | **1.67 ms** | **1.39 ms** | **704.4 ms** | **111000.0000** |     **-** |     **-** | **87636256 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |            NuGetCopy | 452.4 ms | 2.07 ms | 1.93 ms | 451.9 ms |  20000.0000 |     - |     - | 87373056 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |            NuGetCopy | 483.0 ms | 6.37 ms | 5.96 ms | 485.0 ms |  20000.0000 |     - |     - | 87373720 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |           **NuGetCopy2** | **701.2 ms** | **1.60 ms** | **1.42 ms** | **701.2 ms** | **111000.0000** |     **-** |     **-** | **87636256 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |           NuGetCopy2 | 448.7 ms | 1.83 ms | 1.62 ms | 448.6 ms |  20000.0000 |     - |     - | 87373056 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |           NuGetCopy2 | 488.0 ms | 1.45 ms | 1.21 ms | 488.2 ms |  20000.0000 |     - |     - | 87373752 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |           **NuGetCopy3** | **450.6 ms** | **1.43 ms** | **1.12 ms** | **451.1 ms** |           **-** |     **-** |     **-** |     **8192 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |           NuGetCopy3 | 265.2 ms | 4.95 ms | 4.63 ms | 263.5 ms |           - |     - |     - |       64 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |           NuGetCopy3 | 318.7 ms | 2.18 ms | 2.04 ms | 317.9 ms |           - |     - |     - |      600 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |           **NuGetCopy4** | **427.9 ms** | **1.81 ms** | **1.69 ms** | **427.8 ms** |           **-** |     **-** |     **-** |     **8192 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |           NuGetCopy4 | 252.6 ms | 2.02 ms | 1.89 ms | 252.1 ms |           - |     - |     - |       64 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |           NuGetCopy4 | 286.7 ms | 2.19 ms | 2.05 ms | 286.0 ms |           - |     - |     - |      600 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |           **NuGetCopy5** | **425.3 ms** | **2.20 ms** | **1.95 ms** | **425.5 ms** |           **-** |     **-** |     **-** |     **8192 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |           NuGetCopy5 | 245.6 ms | 1.42 ms | 1.19 ms | 245.3 ms |           - |     - |     - |       64 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |           NuGetCopy5 | 293.5 ms | 1.59 ms | 1.41 ms | 293.6 ms |           - |     - |     - |      600 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |           **NuGetCopy6** | **426.6 ms** | **1.24 ms** | **1.16 ms** | **426.5 ms** |           **-** |     **-** |     **-** |     **8192 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |           NuGetCopy6 | 247.5 ms | 1.74 ms | 1.54 ms | 247.5 ms |           - |     - |     - |       64 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |           NuGetCopy6 | 281.0 ms | 2.10 ms | 1.96 ms | 280.9 ms |           - |     - |     - |      600 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |           **NuGetCopy7** | **403.2 ms** | **1.53 ms** | **1.35 ms** | **403.2 ms** |           **-** |     **-** |     **-** |     **8192 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |           NuGetCopy7 | 227.5 ms | 1.71 ms | 1.60 ms | 227.4 ms |           - |     - |     - |       64 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |           NuGetCopy7 | 269.3 ms | 1.51 ms | 1.41 ms | 269.5 ms |           - |     - |     - |      600 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |    **VersionWithString** | **400.2 ms** | **1.98 ms** | **1.76 ms** | **399.9 ms** |           **-** |     **-** |     **-** |     **8192 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |    VersionWithString | 235.3 ms | 2.05 ms | 1.92 ms | 235.0 ms |           - |     - |     - |      360 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |    VersionWithString | 233.8 ms | 2.45 ms | 2.17 ms | 232.9 ms |           - |     - |     - |      352 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |     **VersionWithClass** | **215.8 ms** | **2.82 ms** | **2.63 ms** | **216.2 ms** |           **-** |     **-** |     **-** |     **8192 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |     VersionWithClass | 193.3 ms | 2.76 ms | 2.31 ms | 192.3 ms |           - |     - |     - |       64 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |     VersionWithClass | 187.2 ms | 1.11 ms | 0.98 ms | 187.4 ms |           - |     - |     - |      352 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |    **VersionWithStruct** | **228.4 ms** | **4.47 ms** | **5.97 ms** | **227.3 ms** |           **-** |     **-** |     **-** |     **8192 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |    VersionWithStruct | 185.2 ms | 3.56 ms | 5.22 ms | 182.5 ms |           - |     - |     - |       64 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |    VersionWithStruct | 193.5 ms | 1.30 ms | 1.01 ms | 193.4 ms |           - |     - |     - |      352 B |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** | **VersionWithTwoArrays** | **220.3 ms** | **1.73 ms** | **1.53 ms** | **220.2 ms** |           **-** |     **-** |     **-** |     **8192 B** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 | VersionWithTwoArrays | 186.7 ms | 1.31 ms | 1.23 ms | 186.5 ms |           - |     - |     - |       64 B |
|   Sort | .NET Core 5.0 | .NET Core 5.0 | VersionWithTwoArrays | 193.9 ms | 1.35 ms | 1.26 ms | 193.6 ms |           - |     - |     - |      352 B |
