namespace Trarizon.Library.Collections.CollectionQuery;
public static partial class ReadOnlyListQuery
{
    public static IEnumerable<T> ReverseList<T>(this IReadOnlyList<T> list) => new ReverseIterator<T>(list);

    public static T LastValue<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
    {
        for (int i = list.Count - 1; i >= 0; i--) {
            if (predicate(list[i])) {
                return list[i];
            }
        }
        throw new InvalidOperationException("No matched value");
    }

    public static T? LastOrDefaultValue<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
    {
        for (int i = list.Count - 1; i >= 0; i--) {
            if (predicate(list[i])) {
                return list[i];
            }
        }
        return default;
    }
}
