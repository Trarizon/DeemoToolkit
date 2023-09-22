using Trarizon.Library.CLParsing.Exceptions;
using Trarizon.Library.CLParsing.Utility;

namespace Trarizon.Library.CLParsing.Converters;
internal static class InputConverter
{
    public static object? Convert(Type type, ReadOnlySpan<char> text)
        => Convert(type, text.AsString());

    public static object? Convert(Type type, string text)
    {
        if (type == typeof(string))
            return text;
        if (Nullable.GetUnderlyingType(type) is Type nullableUnderlyingType)
            type = nullableUnderlyingType;
        if (ReflectionUtil.TryParseText(type, text, out var res))
            return res;
        return Throw.RegulationInitializeFailed<object?>("Parameter type must be string or IParsable<>");
    }

    public static object ConvertToArray(Type elementType, ReadOnlySpan<string> texts)
    {
        if (texts.Length == 0)
            return ReflectionUtil.GetEmptyArray(elementType);

        Array array = Array.CreateInstance(elementType, texts.Length);
        for (int i = 0; i < texts.Length; i++) {
            array.SetValue(Convert(elementType, texts[i]), i);
        }
        return array;
    }

    public static object ConvertToList(Type elementType, ReadOnlySpan<string> texts)
    {
        object list = Activator.CreateInstance(ReflectionUtil.GetGenericListType(elementType))!;
        for (int i = 0; i < texts.Length; i++) {
            ReflectionUtil.AddToList(list, elementType, Convert(elementType, texts[i]));
        }
        return list;
    }
}
