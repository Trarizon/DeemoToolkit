namespace Trarizon.Library.CLParsing.Utility;
internal static class StringUtil
{
    public static string AsString(this ReadOnlySpan<char> input) 
        => new(input);

    public static bool IsParameterNameValid(this string? parameterName)
    {
        if (parameterName == null)
            return true;

        foreach (char c in parameterName) {
            if (char.IsWhiteSpace(c) || c == CLParser.PairSeperator)
                return false;
        }

        return true;
    }

    public static string Unescape(this ReadOnlySpan<char> escapedInput)
    {
        Span<char> buffer = stackalloc char[escapedInput.Length];
        int count = 0;
        for (int i = 0; i < escapedInput.Length; i++) {
            if (escapedInput[i..].StartsWith("\"\""))
                count++;
            buffer[count++] = escapedInput[i];
        }
        return AsString(buffer[..count]);
    }
}
