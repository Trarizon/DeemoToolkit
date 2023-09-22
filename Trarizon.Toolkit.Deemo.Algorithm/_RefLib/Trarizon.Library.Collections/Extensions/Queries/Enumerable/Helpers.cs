﻿namespace Trarizon.Library.Collections.Extensions.Queries;
public static partial class EnumerableQuery
{
    internal static bool TryIterate<T>(this IEnumerator<T> enumerator, int count, out int iteratedCount)
    {
        iteratedCount = 0;
        if (count == 0)
            return true;

        while (enumerator.MoveNext()) {
            if (++iteratedCount >= count)
                return true;
        }

        return false;
    }
}
