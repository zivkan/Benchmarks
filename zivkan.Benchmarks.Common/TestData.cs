using System.IO;

namespace zivkan.Benchmarks.Common
{
    public static class TestData
    {
        public static string NuGetOrgCatalog => "NuGetOrgCatalog";

        public static string GetCachePath()
        {
            var cacheRoot = FindGitIgnoreDirectory();
            if (cacheRoot == null)
            {
                cacheRoot = Path.GetTempPath();
            }

            var cachePath = Path.Combine(cacheRoot, "TestData");
            return cachePath;
        }

        private static string? FindGitIgnoreDirectory()
        {
            var directory = Path.GetDirectoryName(typeof(TestData).Assembly.Location);
            if (directory == null)
            {
                return null;
            }

            var gitignore = Path.Combine(directory, ".gitignore");
            while (!File.Exists(gitignore))
            {
                var newDir = Path.GetDirectoryName(directory);
                if (newDir == directory || newDir == null)
                {
                    return null;
                }
                directory = newDir;
                gitignore = Path.Combine(directory, ".gitignore");
            }

            return directory;
        }
    }
}
