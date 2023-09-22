using System.Reflection;

namespace Trarizon.Library.CLParsing.Utility;
internal static class ReflectionUtil
{
    private static readonly MethodInfo EmptyArrayMethod = typeof(Array).GetMethod("Empty")!;
    private static readonly Type[] IParsableParametersTypes = new[] { typeof(string), typeof(IFormatProvider) };
    private static readonly Type[] ParseParametersTypes = new[] { typeof(string) };

    private static readonly CacheDictionary<object> EmptyArrays = new(type => EmptyArrayMethod.MakeGenericMethod(type).InvokeStatic()!);
    private static readonly CacheDictionary<MethodInfo> ListAddMethods = new(type => GetGenericListType(type).GetMethod("Add")!);
    private static readonly CacheDictionary<Type> GenericListTypes = new(type => typeof(List<>).MakeGenericType(type));
    private static readonly CacheDictionary<(MethodInfo, bool RequiresFormatProvider)> ParseMethods = new(type =>
        {
            var method = type.GetMethod("Parse", ParseParametersTypes);
            if (method != null)
                return (method, false);

            method = type.GetMethod("Parse", IParsableParametersTypes);
            if (method != null)
                return (method, true);

            return default;
        });

    public static object? InvokeStatic(this MethodInfo method, params object?[] parameters)
        => method.Invoke(null, parameters);

    public static object GetEmptyArray(Type type) => EmptyArrays.Get(type);

    public static Type GetGenericListType(Type type)=>GenericListTypes.Get(type);

    public static void AddToList(object list, Type elementType, object? value)
    {
        var method = ListAddMethods.Get(elementType);
        method.Invoke(list, new object?[] { value });
    }

    public static bool TryParseText(Type type, string text, out object? result)
    {
        var res = ParseMethods.Get(type);
        if (res == default) {
            result = default;
            return false;
        }
        else {
            result = res.RequiresFormatProvider
                 ? res.Item1.InvokeStatic(text, default(IFormatProvider))
                 : res.Item1.InvokeStatic(text);
            return true;
        }
    }
}
