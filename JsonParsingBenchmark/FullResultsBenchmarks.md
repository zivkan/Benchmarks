``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.200-preview.20614.14
  [Host]        : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
  .NET 4.7.2    : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.11 (CoreCLR 4.700.20.56602, CoreFX 4.700.20.56604), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT


```
|                                 Method |           Job |       Runtime |        InputFile |          Mean |      Error |     StdDev | Ratio | RatioSD |       Gen 0 |       Gen 1 |     Gen 2 |     Allocated |
|--------------------------------------- |-------------- |-------------- |----------------- |--------------:|-----------:|-----------:|------:|--------:|------------:|------------:|----------:|--------------:|
|      **&#39;System.Text.Json JsonSerializer&#39;** |    **.NET 4.7.2** |    **.NET 4.7.2** | **dotnet-core.json** |  **2,658.695 ms** |  **8.7621 ms** |  **7.7673 ms** |  **1.00** |    **0.00** |  **79000.0000** |  **28000.0000** | **1000.0000** |  **478163.17 KB** |
| &#39;System.Text.Json with JsonConverters&#39; |    .NET 4.7.2 |    .NET 4.7.2 | dotnet-core.json |  2,734.494 ms | 12.2137 ms | 10.8272 ms |  1.03 |    0.00 |  80000.0000 |  30000.0000 | 2000.0000 |  996996.89 KB |
|       &#39;Newtonsoft.Json JsonSerializer&#39; |    .NET 4.7.2 |    .NET 4.7.2 | dotnet-core.json |  2,932.021 ms | 57.1279 ms | 53.4375 ms |  1.10 |    0.02 | 120000.0000 |  21000.0000 | 1000.0000 |  723577.64 KB |
|  &#39;Newtonsoft.Json with JsonConverters&#39; |    .NET 4.7.2 |    .NET 4.7.2 | dotnet-core.json |  2,922.579 ms | 10.8906 ms |  9.0942 ms |  1.10 |    0.00 | 119000.0000 |  42000.0000 | 1000.0000 |  723564.23 KB |
|         &#39;Newtonsoft.Json with JTokens&#39; |    .NET 4.7.2 |    .NET 4.7.2 | dotnet-core.json | 10,703.492 ms | 41.3420 ms | 36.6486 ms |  4.03 |    0.01 | 440000.0000 | 160000.0000 | 4000.0000 |  2653970.2 KB |
|                                        |               |               |                  |               |            |            |       |         |             |             |           |               |
|      &#39;System.Text.Json JsonSerializer&#39; | .NET Core 3.1 | .NET Core 3.1 | dotnet-core.json |  2,343.662 ms |  7.0512 ms |  5.5051 ms |  1.00 |    0.00 |  77000.0000 |  27000.0000 | 1000.0000 |  468543.94 KB |
| &#39;System.Text.Json with JsonConverters&#39; | .NET Core 3.1 | .NET Core 3.1 | dotnet-core.json |  2,044.632 ms |  9.6334 ms |  8.5397 ms |  0.87 |    0.00 |  78000.0000 |  29000.0000 | 2000.0000 |  987924.81 KB |
|       &#39;Newtonsoft.Json JsonSerializer&#39; | .NET Core 3.1 | .NET Core 3.1 | dotnet-core.json |  2,721.084 ms | 10.5882 ms |  8.8416 ms |  1.16 |    0.00 | 114000.0000 |  19000.0000 | 1000.0000 |   696138.7 KB |
|  &#39;Newtonsoft.Json with JsonConverters&#39; | .NET Core 3.1 | .NET Core 3.1 | dotnet-core.json |  2,750.799 ms | 11.9933 ms | 11.2185 ms |  1.17 |    0.00 | 114000.0000 |  43000.0000 | 1000.0000 |  696137.62 KB |
|         &#39;Newtonsoft.Json with JTokens&#39; | .NET Core 3.1 | .NET Core 3.1 | dotnet-core.json | 10,346.051 ms | 98.3381 ms | 91.9855 ms |  4.41 |    0.05 | 427000.0000 | 154000.0000 | 4000.0000 | 2594433.94 KB |
|                                        |               |               |                  |               |            |            |       |         |             |             |           |               |
|      &#39;System.Text.Json JsonSerializer&#39; | .NET Core 5.0 | .NET Core 5.0 | dotnet-core.json |  1,870.643 ms | 16.7680 ms | 14.8644 ms |  1.00 |    0.00 |  77000.0000 |  27000.0000 | 1000.0000 |  467280.49 KB |
| &#39;System.Text.Json with JsonConverters&#39; | .NET Core 5.0 | .NET Core 5.0 | dotnet-core.json |  1,958.027 ms | 13.7187 ms | 12.8324 ms |  1.05 |    0.01 |  78000.0000 |  29000.0000 | 2000.0000 |  987924.49 KB |
|       &#39;Newtonsoft.Json JsonSerializer&#39; | .NET Core 5.0 | .NET Core 5.0 | dotnet-core.json |  2,655.401 ms |  8.1779 ms |  7.6496 ms |  1.42 |    0.01 | 114000.0000 |  19000.0000 | 1000.0000 |  696137.38 KB |
|  &#39;Newtonsoft.Json with JsonConverters&#39; | .NET Core 5.0 | .NET Core 5.0 | dotnet-core.json |  2,624.902 ms |  8.6562 ms |  7.6735 ms |  1.40 |    0.01 | 114000.0000 |  43000.0000 | 1000.0000 |   696137.6 KB |
|         &#39;Newtonsoft.Json with JTokens&#39; | .NET Core 5.0 | .NET Core 5.0 | dotnet-core.json | 10,008.273 ms | 34.9901 ms | 31.0178 ms |  5.35 |    0.04 | 429000.0000 | 155000.0000 | 4000.0000 | 2603222.46 KB |
|                                        |               |               |                  |               |            |            |       |         |             |             |           |               |
|      **&#39;System.Text.Json JsonSerializer&#39;** |    **.NET 4.7.2** |    **.NET 4.7.2** |   **nuget-org.json** |      **2.505 ms** |  **0.0124 ms** |  **0.0116 ms** |  **1.00** |    **0.00** |     **74.2188** |     **35.1563** |         **-** |     **448.25 KB** |
| &#39;System.Text.Json with JsonConverters&#39; |    .NET 4.7.2 |    .NET 4.7.2 |   nuget-org.json |      2.517 ms |  0.0080 ms |  0.0075 ms |  1.00 |    0.00 |     74.2188 |     35.1563 |         - |      446.2 KB |
|       &#39;Newtonsoft.Json JsonSerializer&#39; |    .NET 4.7.2 |    .NET 4.7.2 |   nuget-org.json |      2.905 ms |  0.0101 ms |  0.0095 ms |  1.16 |    0.01 |    140.6250 |     66.4063 |         - |     693.07 KB |
|  &#39;Newtonsoft.Json with JsonConverters&#39; |    .NET 4.7.2 |    .NET 4.7.2 |   nuget-org.json |      1.957 ms |  0.0037 ms |  0.0033 ms |  0.78 |    0.00 |    113.2813 |     54.6875 |         - |     693.32 KB |
|         &#39;Newtonsoft.Json with JTokens&#39; |    .NET 4.7.2 |    .NET 4.7.2 |   nuget-org.json |      7.338 ms |  0.0113 ms |  0.0088 ms |  2.93 |    0.01 |    437.5000 |    218.7500 |         - |     2616.1 KB |
|                                        |               |               |                  |               |            |            |       |         |             |             |           |               |
|      &#39;System.Text.Json JsonSerializer&#39; | .NET Core 3.1 | .NET Core 3.1 |   nuget-org.json |      2.224 ms |  0.0064 ms |  0.0056 ms |  1.00 |    0.00 |     70.3125 |     35.1563 |         - |     431.83 KB |
| &#39;System.Text.Json with JsonConverters&#39; | .NET Core 3.1 | .NET Core 3.1 |   nuget-org.json |      1.614 ms |  0.0046 ms |  0.0043 ms |  0.73 |    0.00 |     70.3125 |     35.1563 |         - |     428.67 KB |
|       &#39;Newtonsoft.Json JsonSerializer&#39; | .NET Core 3.1 | .NET Core 3.1 |   nuget-org.json |      2.817 ms |  0.0563 ms |  0.0602 ms |  1.27 |    0.03 |    132.8125 |     54.6875 |         - |     664.05 KB |
|  &#39;Newtonsoft.Json with JsonConverters&#39; | .NET Core 3.1 | .NET Core 3.1 |   nuget-org.json |      1.902 ms |  0.0093 ms |  0.0087 ms |  0.86 |    0.00 |    109.3750 |     54.6875 |         - |     664.31 KB |
|         &#39;Newtonsoft.Json with JTokens&#39; | .NET Core 3.1 | .NET Core 3.1 |   nuget-org.json |      6.954 ms |  0.0268 ms |  0.0250 ms |  3.13 |    0.01 |    437.5000 |    218.7500 |         - |    2546.39 KB |
|                                        |               |               |                  |               |            |            |       |         |             |             |           |               |
|      &#39;System.Text.Json JsonSerializer&#39; | .NET Core 5.0 | .NET Core 5.0 |   nuget-org.json |      1.695 ms |  0.0046 ms |  0.0041 ms |  1.00 |    0.00 |     68.3594 |     33.2031 |         - |     430.27 KB |
| &#39;System.Text.Json with JsonConverters&#39; | .NET Core 5.0 | .NET Core 5.0 |   nuget-org.json |      1.569 ms |  0.0048 ms |  0.0045 ms |  0.93 |    0.00 |     68.3594 |     33.2031 |         - |      428.2 KB |
|       &#39;Newtonsoft.Json JsonSerializer&#39; | .NET Core 5.0 | .NET Core 5.0 |   nuget-org.json |      2.715 ms |  0.0048 ms |  0.0045 ms |  1.60 |    0.00 |    132.8125 |     50.7813 |         - |     664.05 KB |
|  &#39;Newtonsoft.Json with JsonConverters&#39; | .NET Core 5.0 | .NET Core 5.0 |   nuget-org.json |      1.826 ms |  0.0038 ms |  0.0036 ms |  1.08 |    0.00 |    107.4219 |     52.7344 |         - |     664.31 KB |
|         &#39;Newtonsoft.Json with JTokens&#39; | .NET Core 5.0 | .NET Core 5.0 |   nuget-org.json |      6.680 ms |  0.0292 ms |  0.0244 ms |  3.94 |    0.02 |    429.6875 |    210.9375 |         - |    2557.79 KB |
