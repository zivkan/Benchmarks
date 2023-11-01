```

BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 11 (10.0.22631.2428)
AMD Ryzen 9 7950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100-rc.2.23502.2
  [Host]               : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
  .NET 6.0             : .NET 6.0.24 (6.0.2423.51814), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
  .NET Framework 4.8.1 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256


```
| Method                                            | Runtime              | InputFile        | Mean           | StdDev       | Ratio | Gen0        | Gen1        | Gen2      | Allocated     | Alloc Ratio |
|-------------------------------------------------- |--------------------- |----------------- |---------------:|-------------:|------:|------------:|------------:|----------:|--------------:|------------:|
| **&#39;System.Text.Json JsonSerializer&#39;**                 | **.NET 6.0**             | **dotnet-core.json** |   **874,234.7 μs** |  **9,688.32 μs** |  **1.00** |  **31000.0000** |  **17000.0000** | **3000.0000** |  **469286.86 KB** |        **1.00** |
| &#39;System.Text.Json with JsonConverters&#39;            | .NET 6.0             | dotnet-core.json |   804,613.1 μs |  9,471.73 μs |  0.92 |  31000.0000 |  17000.0000 | 3000.0000 |  466427.25 KB |        0.99 |
| &#39;System.Text.Json with JsonDocument&#39;              | .NET 6.0             | dotnet-core.json |   866,566.5 μs |  6,441.73 μs |  0.99 |  30000.0000 |  16000.0000 | 3000.0000 |  450434.69 KB |        0.96 |
| &#39;System.Text.Json with JsonDocument with Cloning&#39; | .NET 6.0             | dotnet-core.json |   952,132.1 μs |  1,980.74 μs |  1.09 |  30000.0000 |  16000.0000 | 3000.0000 |  816123.82 KB |        1.74 |
| &#39;Newtonsoft.Json JsonSerializer&#39;                  | .NET 6.0             | dotnet-core.json | 1,085,400.7 μs | 18,073.27 μs |  1.24 |  44000.0000 |  19000.0000 | 3000.0000 |  670747.07 KB |        1.43 |
| &#39;Newtonsoft.Json with JsonConverters&#39;             | .NET 6.0             | dotnet-core.json | 1,262,648.7 μs |  8,372.09 μs |  1.44 |  45000.0000 |  25000.0000 | 4000.0000 |  670801.77 KB |        1.43 |
| &#39;Newtonsoft.Json with JTokens&#39;                    | .NET 6.0             | dotnet-core.json | 5,838,451.3 μs | 67,658.63 μs |  6.68 | 162000.0000 |  85000.0000 | 6000.0000 | 2554486.06 KB |        5.44 |
|                                                   |                      |                  |                |              |       |             |             |           |               |             |
| &#39;System.Text.Json JsonSerializer&#39;                 | .NET 8.0             | dotnet-core.json |   702,784.3 μs |  7,748.20 μs |  1.00 |  32000.0000 |  31000.0000 | 4000.0000 |  469414.37 KB |        1.00 |
| &#39;System.Text.Json with JsonConverters&#39;            | .NET 8.0             | dotnet-core.json |   629,062.0 μs | 11,046.69 μs |  0.90 |  32000.0000 |  31000.0000 | 4000.0000 |   466428.6 KB |        0.99 |
| &#39;System.Text.Json with JsonDocument&#39;              | .NET 8.0             | dotnet-core.json |   743,738.4 μs | 16,321.36 μs |  1.06 |  31000.0000 |  30000.0000 | 4000.0000 |  450424.68 KB |        0.96 |
| &#39;System.Text.Json with JsonDocument with Cloning&#39; | .NET 8.0             | dotnet-core.json |   759,751.1 μs | 10,401.28 μs |  1.08 |  31000.0000 |  30000.0000 | 4000.0000 |  816114.82 KB |        1.74 |
| &#39;Newtonsoft.Json JsonSerializer&#39;                  | .NET 8.0             | dotnet-core.json |   926,415.0 μs |  6,220.97 μs |  1.32 |  44000.0000 |  23000.0000 | 4000.0000 |  670742.93 KB |        1.43 |
| &#39;Newtonsoft.Json with JsonConverters&#39;             | .NET 8.0             | dotnet-core.json | 1,048,030.7 μs | 10,645.92 μs |  1.49 |  46000.0000 |  45000.0000 | 5000.0000 |  670797.61 KB |        1.43 |
| &#39;Newtonsoft.Json with JTokens&#39;                    | .NET 8.0             | dotnet-core.json | 4,597,778.5 μs | 24,332.90 μs |  6.54 | 163000.0000 | 162000.0000 | 7000.0000 | 2554483.21 KB |        5.44 |
|                                                   |                      |                  |                |              |       |             |             |           |               |             |
| &#39;System.Text.Json JsonSerializer&#39;                 | .NET Framework 4.8.1 | dotnet-core.json | 1,645,433.0 μs | 23,123.94 μs |  1.00 | 101000.0000 |  38000.0000 | 4000.0000 |  597643.44 KB |        1.00 |
| &#39;System.Text.Json with JsonConverters&#39;            | .NET Framework 4.8.1 | dotnet-core.json | 1,437,153.0 μs |  9,593.82 μs |  0.87 |  85000.0000 |  35000.0000 | 8000.0000 |  997810.11 KB |        1.67 |
| &#39;System.Text.Json with JsonDocument&#39;              | .NET Framework 4.8.1 | dotnet-core.json | 1,718,904.6 μs |  9,673.10 μs |  1.05 |  80000.0000 |  32000.0000 | 6000.0000 |  924958.23 KB |        1.55 |
| &#39;System.Text.Json with JsonDocument with Cloning&#39; | .NET Framework 4.8.1 | dotnet-core.json | 1,861,652.7 μs |  9,654.40 μs |  1.13 |  80000.0000 |  32000.0000 | 6000.0000 | 1290638.22 KB |        2.16 |
| &#39;Newtonsoft.Json JsonSerializer&#39;                  | .NET Framework 4.8.1 | dotnet-core.json | 1,179,979.8 μs |  7,450.87 μs |  0.72 | 117000.0000 |  30000.0000 | 5000.0000 |  698605.38 KB |        1.17 |
| &#39;Newtonsoft.Json with JsonConverters&#39;             | .NET Framework 4.8.1 | dotnet-core.json | 1,431,197.7 μs |  9,567.33 μs |  0.87 | 118000.0000 |  43000.0000 | 5000.0000 |  698231.31 KB |        1.17 |
| &#39;Newtonsoft.Json with JTokens&#39;                    | .NET Framework 4.8.1 | dotnet-core.json | 5,712,035.6 μs | 87,071.97 μs |  3.47 | 430000.0000 | 153000.0000 | 7000.0000 | 2605152.88 KB |        4.36 |
|                                                   |                      |                  |                |              |       |             |             |           |               |             |
| **&#39;System.Text.Json JsonSerializer&#39;**                 | **.NET 6.0**             | **nuget-org.json**   |     **1,111.5 μs** |      **2.49 μs** |  **1.00** |     **37.1094** |     **17.5781** |         **-** |     **609.54 KB** |        **1.00** |
| &#39;System.Text.Json with JsonConverters&#39;            | .NET 6.0             | nuget-org.json   |     1,242.7 μs |      6.90 μs |  1.12 |     37.1094 |     15.6250 |         - |     606.03 KB |        0.99 |
| &#39;System.Text.Json with JsonDocument&#39;              | .NET 6.0             | nuget-org.json   |       922.1 μs |      2.46 μs |  0.83 |     35.1563 |     16.6016 |         - |     580.77 KB |        0.95 |
| &#39;System.Text.Json with JsonDocument with Cloning&#39; | .NET 6.0             | nuget-org.json   |       987.9 μs |      4.18 μs |  0.89 |    175.7813 |    169.9219 |  140.6250 |    1069.39 KB |        1.75 |
| &#39;Newtonsoft.Json JsonSerializer&#39;                  | .NET 6.0             | nuget-org.json   |     1,612.7 μs |     10.72 μs |  1.45 |     54.6875 |     19.5313 |         - |        922 KB |        1.51 |
| &#39;Newtonsoft.Json with JsonConverters&#39;             | .NET 6.0             | nuget-org.json   |     1,239.2 μs |     10.17 μs |  1.12 |     54.6875 |     27.3438 |         - |      922.7 KB |        1.51 |
| &#39;Newtonsoft.Json with JTokens&#39;                    | .NET 6.0             | nuget-org.json   |     3,436.2 μs |     13.45 μs |  3.09 |    218.7500 |    109.3750 |         - |    3602.17 KB |        5.91 |
|                                                   |                      |                  |                |              |       |             |             |           |               |             |
| &#39;System.Text.Json JsonSerializer&#39;                 | .NET 8.0             | nuget-org.json   |       925.3 μs |      9.81 μs |  1.00 |     37.1094 |     27.3438 |         - |     609.77 KB |        1.00 |
| &#39;System.Text.Json with JsonConverters&#39;            | .NET 8.0             | nuget-org.json   |     1,008.7 μs |     14.53 μs |  1.09 |     35.1563 |     31.2500 |         - |     606.07 KB |        0.99 |
| &#39;System.Text.Json with JsonDocument&#39;              | .NET 8.0             | nuget-org.json   |       848.6 μs |     20.12 μs |  0.92 |     35.1563 |     19.5313 |         - |     580.77 KB |        0.95 |
| &#39;System.Text.Json with JsonDocument with Cloning&#39; | .NET 8.0             | nuget-org.json   |       930.8 μs |     15.90 μs |  1.01 |    175.7813 |    166.0156 |  140.6250 |    1069.37 KB |        1.75 |
| &#39;Newtonsoft.Json JsonSerializer&#39;                  | .NET 8.0             | nuget-org.json   |     1,165.8 μs |      8.85 μs |  1.26 |     54.6875 |     25.3906 |         - |     922.02 KB |        1.51 |
| &#39;Newtonsoft.Json with JsonConverters&#39;             | .NET 8.0             | nuget-org.json   |     1,007.5 μs |     12.88 μs |  1.09 |     54.6875 |     37.1094 |         - |     922.72 KB |        1.51 |
| &#39;Newtonsoft.Json with JTokens&#39;                    | .NET 8.0             | nuget-org.json   |     2,314.1 μs |     22.06 μs |  2.50 |    218.7500 |    199.2188 |         - |    3602.22 KB |        5.91 |
|                                                   |                      |                  |                |              |       |             |             |           |               |             |
| &#39;System.Text.Json JsonSerializer&#39;                 | .NET Framework 4.8.1 | nuget-org.json   |     1,862.8 μs |     11.08 μs |  1.00 |    138.6719 |     58.5938 |         - |     862.41 KB |        1.00 |
| &#39;System.Text.Json with JsonConverters&#39;            | .NET Framework 4.8.1 | nuget-org.json   |     2,383.5 μs |     12.50 μs |  1.28 |    101.5625 |     50.7813 |         - |     632.73 KB |        0.73 |
| &#39;System.Text.Json with JsonDocument&#39;              | .NET Framework 4.8.1 | nuget-org.json   |     2,553.8 μs |     16.64 μs |  1.37 |     97.6563 |     27.3438 |         - |     602.17 KB |        0.70 |
| &#39;System.Text.Json with JsonDocument with Cloning&#39; | .NET Framework 4.8.1 | nuget-org.json   |     2,620.7 μs |     21.47 μs |  1.41 |    238.2813 |    222.6563 |  140.6250 |    1091.59 KB |        1.27 |
| &#39;Newtonsoft.Json JsonSerializer&#39;                  | .NET Framework 4.8.1 | nuget-org.json   |     1,779.2 μs |     10.06 μs |  0.96 |    156.2500 |     58.5938 |         - |     962.34 KB |        1.12 |
| &#39;Newtonsoft.Json with JsonConverters&#39;             | .NET Framework 4.8.1 | nuget-org.json   |     1,397.4 μs |     12.39 μs |  0.75 |    156.2500 |     58.5938 |         - |     963.18 KB |        1.12 |
| &#39;Newtonsoft.Json with JTokens&#39;                    | .NET Framework 4.8.1 | nuget-org.json   |     4,318.4 μs |     16.81 μs |  2.32 |    593.7500 |    296.8750 |         - |    3684.79 KB |        4.27 |
