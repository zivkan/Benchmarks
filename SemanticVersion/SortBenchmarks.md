```

BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 11 (10.0.22631.2428)
AMD Ryzen 9 7950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100-rc.2.23502.2
  [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
  Job-REIZUU : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
  Job-OWRWNV : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
  Job-VLAXIP : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256

InvocationCount=1  UnrollFactor=1  

```
| Method | Runtime              | Implementation       | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0       | Allocated   | Alloc Ratio |
|------- |--------------------- |--------------------- |---------:|--------:|--------:|------:|--------:|-----------:|------------:|------------:|
| **Sort**   | **.NET 6.0**             | **NuGet**                | **274.6 ms** | **2.11 ms** | **1.98 ms** |  **1.19** |    **0.02** |          **-** |      **1232 B** |        **1.13** |
| Sort   | .NET 8.0             | NuGet                | 230.3 ms | 4.53 ms | 4.24 ms |  1.00 |    0.00 |          - |      1088 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | NuGet                | 476.6 ms | 2.96 ms | 2.62 ms |  2.07 |    0.03 |          - |           - |        0.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **NuGetCopy**            | **352.6 ms** | **2.77 ms** | **2.46 ms** |  **1.11** |    **0.01** | **10000.0000** | **177544528 B** |        **1.00** |
| Sort   | .NET 8.0             | NuGetCopy            | 316.5 ms | 3.59 ms | 3.18 ms |  1.00 |    0.00 | 10000.0000 | 177544384 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | NuGetCopy            | 671.2 ms | 5.04 ms | 4.72 ms |  2.12 |    0.03 | 28000.0000 | 178071184 B |        1.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **NuGetCopy2**           | **377.0 ms** | **4.38 ms** | **3.88 ms** |  **1.18** |    **0.02** | **10000.0000** | **177544528 B** |        **1.00** |
| Sort   | .NET 8.0             | NuGetCopy2           | 320.7 ms | 2.84 ms | 2.37 ms |  1.00 |    0.00 | 10000.0000 | 177544384 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | NuGetCopy2           | 681.8 ms | 3.94 ms | 3.68 ms |  2.13 |    0.01 | 28000.0000 | 178071184 B |        1.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **NuGetCopy3**           | **267.3 ms** | **2.13 ms** | **1.99 ms** |  **1.10** |    **0.02** |          **-** |      **1232 B** |        **1.13** |
| Sort   | .NET 8.0             | NuGetCopy3           | 243.8 ms | 3.76 ms | 3.52 ms |  1.00 |    0.00 |          - |      1088 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | NuGetCopy3           | 494.3 ms | 4.86 ms | 4.54 ms |  2.03 |    0.03 |          - |           - |        0.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **NuGetCopy4**           | **245.2 ms** | **2.52 ms** | **2.35 ms** |  **1.08** |    **0.01** |          **-** |      **1232 B** |        **1.13** |
| Sort   | .NET 8.0             | NuGetCopy4           | 226.3 ms | 0.96 ms | 0.75 ms |  1.00 |    0.00 |          - |      1088 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | NuGetCopy4           | 465.0 ms | 5.12 ms | 4.54 ms |  2.05 |    0.02 |          - |           - |        0.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **NuGetCopy5**           | **250.3 ms** | **2.10 ms** | **1.97 ms** |  **1.13** |    **0.01** |          **-** |      **2344 B** |        **2.15** |
| Sort   | .NET 8.0             | NuGetCopy5           | 221.2 ms | 1.54 ms | 1.37 ms |  1.00 |    0.00 |          - |      1088 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | NuGetCopy5           | 460.2 ms | 5.29 ms | 4.95 ms |  2.08 |    0.03 |          - |           - |        0.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **NuGetCopy6**           | **273.5 ms** | **3.66 ms** | **3.42 ms** |  **1.22** |    **0.02** |          **-** |      **1232 B** |        **1.13** |
| Sort   | .NET 8.0             | NuGetCopy6           | 223.9 ms | 1.63 ms | 1.52 ms |  1.00 |    0.00 |          - |      1088 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | NuGetCopy6           | 462.8 ms | 2.46 ms | 2.05 ms |  2.07 |    0.01 |          - |           - |        0.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **NuGetCopy7**           | **244.8 ms** | **3.06 ms** | **2.86 ms** |  **1.19** |    **0.01** |          **-** |      **1232 B** |        **1.13** |
| Sort   | .NET 8.0             | NuGetCopy7           | 205.3 ms | 2.02 ms | 1.69 ms |  1.00 |    0.00 |          - |      1088 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | NuGetCopy7           | 441.0 ms | 2.16 ms | 2.02 ms |  2.15 |    0.02 |          - |           - |        0.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **VersionWithString**    | **239.0 ms** | **2.98 ms** | **2.79 ms** |  **1.22** |    **0.01** |          **-** |      **1168 B** |        **1.14** |
| Sort   | .NET 8.0             | VersionWithString    | 196.4 ms | 1.35 ms | 1.12 ms |  1.00 |    0.00 |          - |      1024 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | VersionWithString    | 436.6 ms | 3.10 ms | 2.90 ms |  2.22 |    0.02 |          - |           - |        0.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **VersionWithClass**     | **193.4 ms** | **2.07 ms** | **1.94 ms** |  **1.19** |    **0.02** |          **-** |      **1168 B** |        **1.14** |
| Sort   | .NET 8.0             | VersionWithClass     | 162.9 ms | 2.19 ms | 1.95 ms |  1.00 |    0.00 |          - |      1024 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | VersionWithClass     | 246.3 ms | 3.58 ms | 3.35 ms |  1.51 |    0.03 |          - |           - |        0.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **VersionWithStruct**    | **199.9 ms** | **3.57 ms** | **4.64 ms** |  **1.20** |    **0.04** |          **-** |      **1168 B** |        **1.14** |
| Sort   | .NET 8.0             | VersionWithStruct    | 166.6 ms | 3.28 ms | 3.37 ms |  1.00 |    0.00 |          - |      1024 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | VersionWithStruct    | 250.6 ms | 4.45 ms | 5.79 ms |  1.51 |    0.04 |          - |           - |        0.00 |
|        |                      |                      |          |         |         |       |         |            |             |             |
| **Sort**   | **.NET 6.0**             | **VersionWithTwoArrays** | **198.2 ms** | **3.50 ms** | **3.27 ms** |  **1.15** |    **0.02** |          **-** |      **1168 B** |        **1.14** |
| Sort   | .NET 8.0             | VersionWithTwoArrays | 172.1 ms | 3.37 ms | 2.99 ms |  1.00 |    0.00 |          - |      1024 B |        1.00 |
| Sort   | .NET Framework 4.8.1 | VersionWithTwoArrays | 248.1 ms | 4.09 ms | 5.03 ms |  1.45 |    0.03 |          - |           - |        0.00 |
