using System;

namespace SemanticVersionBenchmarks.Implementations.Parsers
{
    internal static class ParserShared
    {
        public static void ValidateMetadata(string metadata)
        {
            if (metadata.Length == 0)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < metadata.Length; i++)
            {
                char c = metadata[i];
                if (!((c >= '0' && c <= '9')
                    || (c >= 'a' && c <= 'z')
                    || (c >= 'A' && c <= 'Z')
                    || c == '.' || c == '-'))
                {
                    throw new ArgumentException();
                }

                if (c == '.' && (i == 0 || metadata[i - 1] == '.'))
                {
                    throw new ArgumentException();
                }
            }

            if (metadata[metadata.Length - 1] == '.')
            {
                throw new ArgumentException();
            }
        }
    }
}
