namespace Trarizon.Library.CLParsing.Input;
internal sealed class StringInput : IInputArguments
{
    private readonly ReadOnlyMemory<char> _input;
    private readonly List<Split> _splits;
    private int _index;

    public StringInput(ReadOnlyMemory<char> input)
    {
        _input = input;
        _splits = SplitInput(input.Span);
        _index = -1;
    }

    public InputSplit Current => (_splits[_index] is var split && split.Seperator == InputSplit.InQuote)
        ? new InputSplit(_input.Span.Slice(split.Start, split.Length), true)
        : new InputSplit(_input.Span.Slice(split.Start, split.Length), split.Seperator);

    public bool Next() => ++_index < _splits.Count;

    private static List<Split> SplitInput(ReadOnlySpan<char> input)
    {
        int start = 0;
        List<Split> splits = new();

        while (start < input.Length) {
            if (char.IsWhiteSpace(input[start])) {
                start++;
                continue;
            }

            int end = start + 1;
            // Value
            if (input[start] == CLParser.Quote) {
                start++;
                while (end < input.Length) {
                    if (input[end] == CLParser.Quote) {
                        if (end + 1 < input.Length && input[end + 1] == CLParser.Quote)
                            end++;
                        else
                            break; // skip last quote
                    }
                    end++;
                }
                splits.Add(new Split(start, end, InputSplit.InQuote));
            }
            else {
                int seperator = InputSplit.Normal;
                while (end < input.Length && !char.IsWhiteSpace(input[end])) {
                    if (seperator == InputSplit.Normal && input[end] == CLParser.PairSeperator) {
                        seperator = end - start;
                    }
                    end++;
                }
                // Pair
                if (seperator != InputSplit.Normal) {
                    // With quote
                    if (input[seperator + 1] == CLParser.Quote) {
                        end = seperator + 1;
                        while (end < input.Length) {
                            if (input[end] == CLParser.Quote) {
                                end++; // Contains last quote
                                if (end >= input.Length || input[end] != CLParser.Quote)
                                    break;
                            }
                        }
                    }
                    else {
                        while (end < input.Length && !char.IsWhiteSpace(input[end]))
                            end++;
                    }
                }
                splits.Add(new Split(start, end, seperator));
            }
            start = end + 1;
        }

        return splits;
    }

    private readonly struct Split
    {
        public readonly int Start;
        public readonly int Length;
        public readonly int Seperator;

        public Split(int start, int end, int seperator)
        {
            Start = start;
            Length = end - start;
            Seperator = seperator;
        }
    }
}
