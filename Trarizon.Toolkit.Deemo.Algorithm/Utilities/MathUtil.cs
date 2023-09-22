using System.Numerics;

namespace Trarizon.Toolkit.Deemo.Algorithm.Utilities;
internal static class MathUtil
{
	public static T Max<T>(ReadOnlySpan<T> values)
	where T : IComparisonOperators<T, T, bool>
	{
		T max = values[0];
		for (int i = 1; i < values.Length; i++)
			if (values[i] > max)
				max = values[i];
		return max;
	}

	public static T Sum<T>(this (T, T) tuple)
		where T : IAdditionOperators<T, T, T>
		=> tuple.Item1 + tuple.Item2;
}
