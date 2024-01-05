using System;
using System.Linq;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;
internal static class StringExtensions
{
    public static string Replace(this string input, ReadOnlySpan<char> oldChars, char newChar)
    {
        Span<char> result = stackalloc char[input.Length];

        for (int i = 0; i < input.Length; i++) {
            char c = input[i];
            if (oldChars.Contains(c))
                result[i] = newChar;
            else
                result[i] = c;
        }

        return new string(result);
    }
}
