using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;
[InlineArray(4)]
[CollectionBuilder(typeof(ChartAdaptedArray), nameof(ChartAdaptedArray.Create))]
internal struct ChartAdaptedArray<T>
{
    T _val;

    public T this[ChartDifficulty difficulty]
    {
        get => Unsafe.Add(ref Unsafe.As<ChartAdaptedArray<T>, T>(ref this), Unsafe.As<ChartDifficulty, int>(ref difficulty));
        set => Unsafe.Add(ref Unsafe.As<ChartAdaptedArray<T>, T>(ref this), Unsafe.As<ChartDifficulty, int>(ref difficulty)) = value;
    }
}

internal static class ChartAdaptedArray
{
    public static ChartAdaptedArray<T> Create<T>(ReadOnlySpan<T> data)
    {
        Debug.Assert(data.Length <= 4);
        ChartAdaptedArray<T> result = default;
        for (int i = 0; i < data.Length; i++)
            result[i] = data[i];
        return result;
    }
}
