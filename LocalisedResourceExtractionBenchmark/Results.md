```

BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 11 (10.0.22631.2428)
AMD Ryzen 9 7950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100-rc.2.23502.2
  [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2


```
| Method                 | Mean      | Error    | StdDev   | Gen0       | Gen1      | Gen2      | Allocated |
|----------------------- |----------:|---------:|---------:|-----------:|----------:|----------:|----------:|
| BasicJoin              |  93.35 ms | 1.808 ms | 2.286 ms |  3166.6667 | 3000.0000 | 1000.0000 |  38.34 MB |
| BasicJoinAsXml         | 101.45 ms | 2.014 ms | 2.398 ms |  1600.0000 | 1400.0000 |  400.0000 |  22.03 MB |
| LabelsAsXml            | 164.36 ms | 2.584 ms | 2.417 ms | 17666.6667 | 5666.6667 | 2000.0000 | 255.94 MB |
| LabelsAsJoinedColumns  |  76.70 ms | 1.455 ms | 1.494 ms |  2714.2857 | 2571.4286 |  857.1429 |  30.42 MB |
| LabelsAsPivotedColumns | 127.19 ms | 0.274 ms | 0.229 ms |  2500.0000 | 2250.0000 |  750.0000 |  30.42 MB |
| SingleLanguage         |  39.69 ms | 0.661 ms | 0.618 ms |   846.1538 |  769.2308 |  230.7692 |  11.03 MB |
| LocalJoin              |  71.00 ms | 1.357 ms | 1.269 ms |  3250.0000 | 3125.0000 | 1250.0000 |  41.98 MB |
