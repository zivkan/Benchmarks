using NuGet.Versioning;
using SemanticVersionBenchmarks;
using SemanticVersionBenchmarks.Implementations;
using SemanticVersionBenchmarks.Implementations.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SemanticVersionTests
{
    public class ParserTests
    {
        private IReadOnlyList<string> _input;
        private IReadOnlyList<NuGetVersion> _expected;
        private IReadOnlyList<string> _errors;

        public ParserTests()
        {
            ClassSetupAsync().GetAwaiter().GetResult();
        }

        private async Task ClassSetupAsync()
        {
            _input = await VersionData.GetVersionsAsync(false);

            var expected = new List<NuGetVersion>();
            for (int i = 0; i < _input.Count; i++)
            {
                var version = _input[i];
                var parsed = NuGetVersion.Parse(version);
                expected.Add(parsed);
            }

            _expected = expected;

            _errors = new string[]
            {
                "1.",
                "1..2",
                "1.2.3.4.5",
                "1.2.a",
                "1.2.3-",
                "1.2.3-a.",
                "1.2.3-a..b",
                "1.2.3-.a",
                "1.2.3-a!",
                "1.2.3+a.",
                "1.2.3+a..b",
                "1.2.3+a!"
            };
        }

        [Fact]
        public void Substring() => AssertParser(SubstringParser.Parse);

        [Fact]
        public void Span() => AssertParser(SpanParser.Parse);

        [Fact]
        public void Regex() => AssertParser(RegexParser.Parse);

        [Fact]
        public void StateMachine() => AssertParser(StateMachineParser.Parse);

        [Fact]
        public void NuGetVersionErrors()
        {
            foreach (var input in _errors)
            {
                Assert.ThrowsAny<Exception>(() => NuGetVersion.Parse(input));
            }
        }

        [Fact]
        public void SubstringErrors() => AssertErrors(SubstringParser.Parse);

        [Fact]
        public void SpanErrors() => AssertErrors(SpanParser.Parse);

        [Fact]
        public void RegexErrors() => AssertErrors(RegexParser.Parse);

        [Fact]
        public void StateMachineErrors() => AssertErrors(StateMachineParser.Parse);


        private void AssertParser(Func<string, VersionWithClassArray> parse)
        {
            for (int i = 0; i < _input.Count; i++)
            {
                var expected = _expected[i];
                var actual = parse(_input[i]);

                Assert.Equal(expected.Major, (int)actual.Major);
                Assert.Equal(expected.Minor, (int)actual.Minor);
                Assert.Equal(expected.Patch, (int)actual.Patch);
                Assert.Equal(expected.Revision, (int)(actual.Legacy ?? 0));

                Assert.Equal(expected.IsPrerelease, actual.Prerelease != null);
                if (expected.IsPrerelease)
                {
                    Assert.Equal(expected.ReleaseLabels, actual.Prerelease.Select(s => s.Value));
                }

                Assert.Equal(expected.HasMetadata, actual.Metadata != null);
                if (expected.HasMetadata)
                {
                    Assert.Equal(expected.Metadata, actual.Metadata);
                }
            }
        }

        private void AssertErrors(Func<string, VersionWithClassArray> parse)
        {
            List<string> didNotError = null;

            foreach (var input in _errors)
            {
                try
                {
                    var result = parse(input);
                    if (didNotError == null)
                    {
                        didNotError = new List<string>();
                    }
                    didNotError.Add(input);
                }
                catch
                {
                    // expected
                }
            }

            if (didNotError != null)
            {
                var message = "The following inputs did not error: " + string.Join(", ", didNotError);
                throw new Exception(message);
            }
        }
    }
}
