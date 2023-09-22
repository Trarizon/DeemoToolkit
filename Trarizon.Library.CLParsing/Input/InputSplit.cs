namespace Trarizon.Library.CLParsing.Input;
internal readonly ref struct InputSplit
{
    public const int InQuote = -2; // In quote
    public const int Normal = -1;

    public readonly ReadOnlySpan<char> Value;
    public readonly int SeperatorIndex;

    public InputSplit(ReadOnlySpan<char> value, int seperatorIndex)
    {
        Value = value;
        SeperatorIndex = seperatorIndex;
    }

    public InputSplit(ReadOnlySpan<char> value, bool inQuote) :
        this(value, inQuote ? InQuote : Normal)
    { }
}
