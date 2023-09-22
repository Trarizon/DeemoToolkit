using Trarizon.Library.CLParsing.Utility;

namespace Trarizon.Library.CLParsing;
public static class CLUtility
{
    /// <summary>
    /// Split input by space, ignore spaces in quotes,
    /// use "" to escape "
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyList<string> SplitAsArguments(this string input)
    {
        int start = 0;
        List<string> splits = new();

        while (start < input.Length) {
            if (char.IsWhiteSpace(input[start])) {
                start++;
                continue;
            }
            int end = start + 1;
            if (input[start] == '"') {
                start++;
                while (end < input.Length) {
                    if (input[end] == '"') {
                        if (end + 1 < input.Length && input[end + 1] == '"') // escape
                            end++;
                        else // End
                            break;
                    }
                    end++;
                }

                splits.Add(input.AsSpan(start, end - start).Unescape());
            }
            else {
                while (end < input.Length && !char.IsWhiteSpace(input[end]))
                    end++;

                splits.Add(input[start..end]);
            }
            start = end + 1;
        }
        return splits;
    }

}
