using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

[SimpleJob(RunStrategy.Monitoring, warmupCount: 1, targetCount: 3)]
public class PackageExtraction
{
    IReadOnlyList<string>? packages;
    string extractDirectory;

    [Params(1, 2, 4, 8, 16, 32)]
    public int MaxParallel { get; set; }

    public PackageExtraction()
    {
        extractDirectory = Path.Combine("bin", "extract");
    }

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        var setup = new OrchardCodePackagesSetup();
        packages = await setup.GetPackages();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        if (Directory.Exists(extractDirectory))
        {
            Directory.Delete(extractDirectory, true);
        }
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        Directory.Delete(extractDirectory, true);
    }

    [Benchmark]
    public void CopyTo()
    {
        if (packages == null)
        {
            throw new Exception("Instance hasn't been setup");
        }

        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = MaxParallel
        };

        Parallel.For(0, packages.Count, options, index =>
        {
            ExtractZip(packages[index], CopyTo).GetAwaiter().GetResult();
        });
    }

    private Task CopyTo(Stream input, FileStream output, long length)
    {
        input.CopyTo(output);
        return Task.CompletedTask;
    }

    [Benchmark]
    public async Task CopyToAsync()
    {
        if (packages == null)
        {
            throw new Exception("Instance hasn't been setup");
        }

        var tasks = new Task[MaxParallel];
        var index = 0;
        while (index < tasks.Length)
        {
            var packageIndex = index;
            tasks[index] = Task.Run(() => ExtractZip(packages[packageIndex], CopyToAsync));
            index++;
        }

        while (index < packages.Count)
        {
            await Task.WhenAny(tasks);
            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].IsCompleted)
                {
                    var packageIndex = index;
                    tasks[i] = Task.Run(() => ExtractZip(packages[packageIndex], CopyToAsync));
                    index++;
                }
            }
        }

        await Task.WhenAll(tasks);
    }

    private async Task CopyToAsync(Stream input, FileStream output, long length)
    {
        await input.CopyToAsync(output);
        await output.FlushAsync();
    }

    [Benchmark]
    public void CopyToMmap()
    {
        if (packages == null)
        {
            throw new Exception("Instance hasn't been setup");
        }

        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = MaxParallel
        };

        Parallel.For(0, packages.Count, options, index =>
        {
            ExtractZip(packages[index], CopyToMmap).GetAwaiter().GetResult();
        });
    }

    private Task CopyToMmap(Stream input, FileStream output, long length)
    {
        if (length > 0)
        {
            using (var mmf = MemoryMappedFile.CreateFromFile(output, null, length, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, false))
            using (var mmstream = mmf.CreateViewStream())
            {
                input.CopyTo(mmstream);
            }
        }
        return Task.CompletedTask;
    }

    private async Task ExtractZip(string package, Func<Stream, FileStream, long, Task> copy)
    {
        var destination = Path.Combine(extractDirectory, Path.GetFileNameWithoutExtension(package)) + Path.DirectorySeparatorChar;
        Directory.CreateDirectory(destination);

        using (var zipFile = File.Open(package, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
        using (var zip = new ZipArchive(zipFile))
        {
            foreach (var entry in zip.Entries)
            {
                if (entry.Name.Equals(string.Empty))
                {
                    continue;
                }

                var destinationFile = Path.Combine(destination, entry.FullName);
                var destinationDirectory = Path.GetDirectoryName(destinationFile);
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                if (!destinationFile.StartsWith(destination))
                {
                    throw new Exception("Zip slip");
                }

                using (var writeStream = File.Create(destinationFile))
                using (var zipStream = entry.Open())
                {
                    await copy(zipStream, writeStream, entry.Length);
                }
            }
        }
    }
}
