namespace Trarizon.Toolkit.Deemo.Commands.Utility;
internal static class RandomUtil
{
    public static float NextSingleInArea(this Random random, in Scope scope)
    {
        Scope.Range[] ranges = scope.GetFilledRanges();
        float sum = ranges.Aggregate(0f, (res, range) => res + range.Length);

        var value = random.NextSingle() * sum;
        foreach (var range in ranges) {
            if (range.Length < value)
                value -= range.Length;
            else
                return value + range.Start;
        }

        return ThrowHelper.ThrowInvalidOperationException<float>();
    }
}
