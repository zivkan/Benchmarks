using System;
using System.Collections.Generic;
using System.Text;

namespace SemanticVersionBenchmarks.Implementations.Parsers
{
    public static class StateMachineParser
    {
        public static VersionWithClassArray Parse(string input)
        {
            State state = new State()
            {
                state = Which.NumberSeparator
            };

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                switch (state.state)
                {
                    case Which.NumberSeparator:
                        state.NumberSeparator(c, i);
                        break;

                    case Which.NumberSegment:
                        state.NumberSegment(c, i);
                        break;

                    case Which.PrereleaseSeparator:
                        state.PrereleaseSeperator(c, i);
                        break;

                    case Which.PrereleaseSegment:
                        state.PrereleaseSegment(c, i);
                        break;

                    case Which.MetadataSeparator:
                        state.MetadataSeparator(c, i);
                        break;

                    case Which.Metadata:
                        state.Metadata(c, i);
                        break;

                    default:
                        throw new InvalidOperationException("State " + state.state + " is not valid");
                }
            }

            var version = state.Finish(input);
            return version;
        }

        private enum Which
        {
            NumberSeparator,
            NumberSegment,
            PrereleaseSeparator,
            PrereleaseSegment,
            MetadataSeparator,
            Metadata
        }

        private struct State
        {
            public uint major;
            public uint minor;
            public uint patch;
            public uint? revision;
            public VersionWithClassArray.PrereleaseSegment[]? prerelease;

            public StringBuilder buffer;
            public uint number;
            public List<string>? strings;
            public uint segmentNumber;

            public Which state;

            public void NumberSeparator(char next, int index)
            {
                if (next >= '0' && next <= '9')
                {
                    number = (uint)(next - '0');
                    state = Which.NumberSegment;
                }
                else
                {
                    throw new FormatException("Invalid character '" + next + "'at position " + index + ". Expecting a digit (0-9)");
                }
            }

            public void NumberSegment(char next, int index)
            {
                if (next >= '0' && next <= '9')
                {
                    number = number * 10 + (uint)(next - '0');
                }
                else if (next == '.' || next == '-' || next == '+')
                {
                    switch (segmentNumber)
                    {
                        case 0:
                            major = number;
                            break;

                        case 1:
                            minor = number;
                            break;

                        case 2:
                            patch = number;
                            break;

                        case 3:
                            revision = number;
                            break;

                        default:
                            throw new Exception("bug");
                    }

                    if (next == '.')
                    {
                        segmentNumber++;
                        if (segmentNumber > 3)
                        {
                            throw new FormatException("A version does not have more than 4 number segments (major, minor, patch, revision)");
                        }
                    }

                    state = next switch
                    {
                        '.' => Which.NumberSeparator,
                        '-' => Which.PrereleaseSeparator,
                        '+' => Which.MetadataSeparator,
                        _ => throw new Exception("bug")
                    };
                }
                else
                {
                    throw new FormatException("Invalid character at position " + index + ". Expecting a digit (0-9), '.', '-', or '+'");
                }
            }

            public void PrereleaseSeperator(char next, int index)
            {
                if ((next >= '0' && next <= '9')
                    || (next >= 'a' && next <= 'z')
                    || (next >= 'A' && next <= 'Z')
                    || next == '-')
                {
                    if (buffer == null)
                    {
                        buffer = new StringBuilder();
                    }
                    else
                    {
                        buffer.Clear();
                    }

                    buffer.Append(next);
                    state = Which.PrereleaseSegment;
                }
                else
                {
                    throw new FormatException("Invalid character '" + next + "'in prerelease label at position " + index + ". Expected [0-9a-zA-Z-].");
                }
            }

            public void PrereleaseSegment(char next, int index)
            {
                if ((next >= '0' && next <= '9')
                    || (next >= 'a' && next <= 'z')
                    || (next >= 'A' && next <= 'Z')
                    || next == '-')
                {
                    buffer.Append(next);
                }
                else if (next == '.' || next == '+')
                {
                    var prereleaseSegmentString = buffer.ToString();
                    if (strings == null)
                    {
                        strings = new List<string>();
                    }

                    strings.Add(prereleaseSegmentString);

                    state = next switch
                    {
                        '.' => Which.PrereleaseSeparator,
                        '+' => Which.MetadataSeparator,
                        _ => throw new Exception("bug")
                    };

                    if (state == Which.MetadataSeparator)
                    {
                        prerelease = new VersionWithClassArray.PrereleaseSegment[strings.Count];
                        for (int i = 0; i < strings.Count; i++)
                        {
                            var segment = new VersionWithClassArray.PrereleaseSegment(strings[i]);
                            prerelease[i] = segment;
                        }
                        strings = null;
                        buffer.Clear();
                    }
                }
                else
                {
                    throw new FormatException("Invalid character '" + next + "' at position " + index + ". Expecting [0-9a-zA-Z\\.-+]");
                }
            }

            public void MetadataSeparator(char next, int index)
            {
                if ((next >= '0' && next <= '9')
                    || (next >= 'a' && next <= 'z')
                    || (next >= 'A' && next <= 'Z')
                    || next == '-')
                {
                    if (buffer == null)
                    {
                        buffer = new StringBuilder();
                    }

                    buffer.Append(next);

                    state = Which.Metadata;
                }
                else
                {
                    throw new FormatException();
                }
            }

            public void Metadata(char next, int index)
            {
                if ((next >= '0' && next <= '9')
                    || (next >= 'a' && next <= 'z')
                    || (next >= 'A' && next <= 'Z')
                    || next == '-')
                {
                    buffer.Append(next);
                }
                else if (next == '.')
                {
                    buffer.Append(next);
                    state = Which.MetadataSeparator;
                }
                else
                {
                    throw new FormatException();
                }
            }

            public VersionWithClassArray Finish(string originalString)
            {
                switch (state)
                {
                    case Which.NumberSegment:
                        switch (segmentNumber)
                        {
                            case 0:
                                major = number;
                                return new VersionWithClassArray(originalString, major, 0, 0, null, null, null);

                            case 1:
                                minor = number;
                                return new VersionWithClassArray(originalString, major, minor, 0, null, null, null);

                            case 2:
                                patch = number;
                                return new VersionWithClassArray(originalString, major, minor, patch, null, null, null);

                            case 3:
                                revision = number;
                                return new VersionWithClassArray(originalString, major, minor, patch, revision, null, null);

                            default:
                                throw new Exception("bug");
                        }

                    case Which.PrereleaseSegment:
                        {
                            var prerelaseSegmentString = buffer.ToString();
                            VersionWithClassArray.PrereleaseSegment[] prereleaseSegments;
                            if (strings == null)
                            {
                                prereleaseSegments = new VersionWithClassArray.PrereleaseSegment[1];
                                prereleaseSegments[0] = new VersionWithClassArray.PrereleaseSegment(prerelaseSegmentString);
                            }
                            else
                            {
                                prereleaseSegments = new VersionWithClassArray.PrereleaseSegment[strings.Count + 1];
                                VersionWithClassArray.PrereleaseSegment segment;
                                for (int i = 0; i < strings.Count; i++)
                                {
                                    segment = new VersionWithClassArray.PrereleaseSegment(strings[i]);
                                    prereleaseSegments[i] = segment;
                                }
                                segment = new VersionWithClassArray.PrereleaseSegment(prerelaseSegmentString);
                                prereleaseSegments[prereleaseSegments.Length - 1] = segment;
                            }

                            return new VersionWithClassArray(originalString, major, minor, patch, revision, prereleaseSegments, null);
                        }

                    case Which.Metadata:
                        {
                            var metadataString = buffer.ToString();

                            return new VersionWithClassArray(originalString, major, minor, patch, revision, prerelease, metadataString);
                        }

                    default:
                        throw new Exception("bug");
                }
            }
        }
    }
}
