``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.200-preview.20614.14
  [Host]        : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
  .NET 4.7.2    : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.11 (CoreCLR 4.700.20.56602, CoreFX 4.700.20.56604), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  

```
| Method |           Job |       Runtime |       Implementation |     Mean |   Error |  StdDev |
|------- |-------------- |-------------- |--------------------- |---------:|--------:|--------:|
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |                **NuGet** | **758.2 ms** | **3.45 ms** | **3.06 ms** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |                NuGet | 494.4 ms | 1.96 ms | 1.74 ms |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |                NuGet | 517.2 ms | 2.09 ms | 1.96 ms |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |    **VersionWithString** | **402.9 ms** | **2.11 ms** | **1.98 ms** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |    VersionWithString | 234.7 ms | 2.08 ms | 1.84 ms |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |    VersionWithString | 229.1 ms | 1.63 ms | 1.52 ms |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |     **VersionWithClass** | **239.9 ms** | **2.73 ms** | **2.55 ms** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |     VersionWithClass | 210.0 ms | 1.65 ms | 1.46 ms |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |     VersionWithClass | 203.6 ms | 1.50 ms | 1.33 ms |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** |    **VersionWithStruct** | **241.1 ms** | **0.98 ms** | **0.82 ms** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 |    VersionWithStruct | 208.9 ms | 2.79 ms | 2.33 ms |
|   Sort | .NET Core 5.0 | .NET Core 5.0 |    VersionWithStruct | 202.7 ms | 1.62 ms | 1.44 ms |
|   **Sort** |    **.NET 4.7.2** |    **.NET 4.7.2** | **VersionWithTwoArrays** | **244.9 ms** | **3.14 ms** | **2.62 ms** |
|   Sort | .NET Core 3.1 | .NET Core 3.1 | VersionWithTwoArrays | 211.3 ms | 1.34 ms | 1.19 ms |
|   Sort | .NET Core 5.0 | .NET Core 5.0 | VersionWithTwoArrays | 222.8 ms | 2.31 ms | 2.16 ms |
