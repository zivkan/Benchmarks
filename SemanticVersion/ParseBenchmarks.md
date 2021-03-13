``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.201
  [Host]        : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.13 (CoreCLR 4.700.21.11102, CoreFX 4.700.21.11602), X64 RyuJIT
  .NET 4.7.2    : .NET Framework 4.8 (4.8.4300.0), X64 RyuJIT


```
|       Method |           Job |       Runtime |      Mean |    Error |   StdDev | Ratio | RatioSD |       Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------------- |-------------- |-------------- |----------:|---------:|---------:|------:|--------:|------------:|---------:|---------:|----------:|
| StateMachine | .NET Core 5.0 | .NET Core 5.0 |  64.85 ms | 1.144 ms | 1.014 ms |  0.89 |    0.02 |  18125.0000 | 125.0000 | 125.0000 |  74.91 MB |
|         Span | .NET Core 3.1 | .NET Core 3.1 |  71.54 ms | 0.512 ms | 0.479 ms |  0.98 |    0.02 |  11714.2857 | 142.8571 | 142.8571 |  49.79 MB |
|         Span | .NET Core 5.0 | .NET Core 5.0 |  72.85 ms | 1.192 ms | 1.115 ms |  1.00 |    0.00 |  11857.1429 | 142.8571 | 142.8571 |  49.79 MB |
| StateMachine | .NET Core 3.1 | .NET Core 3.1 |  73.28 ms | 1.352 ms | 1.264 ms |  1.01 |    0.02 |  18142.8571 | 142.8571 | 142.8571 |  74.91 MB |
|         Span |    .NET 4.7.2 |    .NET 4.7.2 |  78.66 ms | 1.526 ms | 2.421 ms |  1.07 |    0.03 |  63714.2857 | 142.8571 | 142.8571 |  50.84 MB |
| StateMachine |    .NET 4.7.2 |    .NET 4.7.2 |  83.54 ms | 0.759 ms | 0.710 ms |  1.15 |    0.02 |  97714.2857 | 142.8571 | 142.8571 |  76.28 MB |
| NuGetVersion | .NET Core 3.1 | .NET Core 3.1 | 101.09 ms | 0.442 ms | 0.413 ms |  1.39 |    0.02 |  20600.0000 |        - |        - |  85.76 MB |
|    SubString | .NET Core 3.1 | .NET Core 3.1 | 104.40 ms | 0.478 ms | 0.447 ms |  1.43 |    0.02 |  24000.0000 |        - |        - |  99.08 MB |
|    SubString | .NET Core 5.0 | .NET Core 5.0 | 105.13 ms | 0.756 ms | 0.670 ms |  1.44 |    0.02 |  24000.0000 |        - |        - |  99.08 MB |
| NuGetVersion | .NET Core 5.0 | .NET Core 5.0 | 106.03 ms | 1.640 ms | 1.370 ms |  1.45 |    0.03 |  20600.0000 |        - |        - |  85.76 MB |
|    SubString |    .NET 4.7.2 |    .NET 4.7.2 | 180.26 ms | 1.647 ms | 1.541 ms |  2.48 |    0.05 | 142333.3333 |        - |        - | 110.06 MB |
| NuGetVersion |    .NET 4.7.2 |    .NET 4.7.2 | 207.31 ms | 0.498 ms | 0.441 ms |  2.85 |    0.05 | 246666.6667 |        - |        - |  188.1 MB |
|        Regex | .NET Core 5.0 | .NET Core 5.0 | 442.83 ms | 2.040 ms | 1.908 ms |  6.08 |    0.09 | 140000.0000 |        - |        - | 563.92 MB |
|        Regex | .NET Core 3.1 | .NET Core 3.1 | 585.40 ms | 3.430 ms | 3.041 ms |  8.04 |    0.14 | 140000.0000 |        - |        - | 563.92 MB |
|        Regex |    .NET 4.7.2 |    .NET 4.7.2 | 630.90 ms | 3.039 ms | 2.843 ms |  8.66 |    0.13 | 786000.0000 |        - |        - | 593.19 MB |
