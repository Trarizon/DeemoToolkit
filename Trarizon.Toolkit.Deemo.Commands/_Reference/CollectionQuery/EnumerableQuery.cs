namespace Trarizon.Library.Collections.CollectionQuery;
public static class EnumerableQuery
{
    public static bool CountsMoreThan<T>(this IEnumerable<T> source, int count)
    {
        if (count <= 0) return true;

        var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext()) {
            if (count > 0) count--;
            else return true;
        }
        return false;
    }

    public static bool CountsMoreThanOrEqualsTo<T>(this IEnumerable<T> source, int count)
        => CountsMoreThan(source, count - 1);
}
