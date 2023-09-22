namespace Trarizon.Library.CLParsing.Utility;
internal static class CollectionUtil
{
    public static bool CountsMoreThan<T>(this IEnumerable<T> source, int count)
        => CountsMoreThan(source, count, out _);

    public static bool CountsMoreThan<T>(this IEnumerable<T> source, int count, out int actualCount)
    {
        actualCount = 0;
        if (count <= 0) return true;

        var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext()) {
            if (actualCount < count) actualCount++;
            else return true;
        }
        return false;
    }
}
