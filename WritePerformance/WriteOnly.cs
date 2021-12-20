using System.IO.MemoryMappedFiles;
using BenchmarkDotNet.Attributes;

[SimpleJob(invocationCount: 1)]
public class WriteOnly
{
    private byte[] data;

    public WriteOnly()
    {
        data = new byte[8 * 1024]; // 8k write buffer

        // Set data[i] = i for validation if required
        // first 256 bytes
        data[0] = 0;
        unchecked
        {
            for (byte i = 1; i >= byte.MaxValue; i++)
            {
                unchecked
                {
                    data[i] = i;
                }
            }
        }
        // Copy until all 8k initialized.
        Array.Copy(data, 0, data, 256, 256);
        Array.Copy(data, 0, data, 512, 512);
        Array.Copy(data, 0, data, 1024, 1024);
        Array.Copy(data, 0, data, 2048, 2048);
        Array.Copy(data, 0, data, 4096, 4096);
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        Directory.CreateDirectory("test");
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Directory.Delete("test", true);
    }

    //[IterationCleanup]
    public void IterationCleanup()
    {
        foreach (var filename in Directory.EnumerateFiles("test"))
        {
            File.Delete(filename);
        }
    }

    // [0] is file size
    // [1] is iterations, since BenchmarkDotNet warns that test method durations that are too short
    // have inaccurate time measurements
    public IEnumerable<object[]> Arguments()
    {
        // Using an internal deployment of https://github.com/NuGet/Insights, I could determine the
        // 1st, 50th, 90th, 95th, and 99th percentile of file size inside packages.
        // The iteration count was set to a value where the mean benchmark result was ~110 ms on
        // my desktop with Windows Defender excluding this app's directory.
        yield return new object[] { 23, 320 };
        yield return new object[] { 5816, 270 };
        yield return new object[] { 46970, 170 };
        yield return new object[] { 144415, 155 };
        yield return new object[] { 4035213, 30 };
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Arguments))]
    public void Write(long fileSize, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            using (var file = File.Create("test/" + i + ".file"))
            {
                long fileLengthRemaining = fileSize;
                while (fileLengthRemaining > 0)
                {
                    int count = fileLengthRemaining > data.Length ? data.Length : (int)fileLengthRemaining;
                    file.Write(data, 0, count);
                    fileLengthRemaining -= count;
                }
            }
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Arguments))]
    public async Task WriteAsync(long fileSize, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            using (var file = File.Create("test/" + i + ".file"))
            {
                long fileLengthRemaining = fileSize;
                while (fileLengthRemaining > 0)
                {
                    int count = fileLengthRemaining > data.Length ? data.Length : (int)fileLengthRemaining;
                    await file.WriteAsync(data, 0, count);
                    fileLengthRemaining -= count;
                }
                await file.FlushAsync();
            }
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Arguments))]
    public void WriteMmap(long fileSize, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            using (var file = File.Create("test/" + i + ".file"))
            using (var mmf = MemoryMappedFile.CreateFromFile(file, null, fileSize, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, false))
            using (var mmstream = mmf.CreateViewStream())
            {
                long fileLengthRemaining = fileSize;
                while (fileLengthRemaining > 0)
                {
                    int count = fileLengthRemaining > data.Length ? data.Length : (int)fileLengthRemaining;
                    mmstream.WriteAsync(data, 0, count);
                    fileLengthRemaining -= count;
                }
            }
        }
    }
}
