namespace Trarizon.Library.CLParsing.Utility;
internal static class CastUtil
{
    private static readonly object TrueObject = true;
    private static readonly object FalseObject = false;

    public static object Box(this bool value)
        => value ? TrueObject : FalseObject;
}
