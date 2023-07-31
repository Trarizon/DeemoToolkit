using System.Diagnostics.CodeAnalysis;

namespace Trarizon.Toolkit.Deemo.Commands.Utility;
internal static class ThrowHelper
{
    [DoesNotReturn]
    public static T ThrowArgumentException<T>(string? paramName = null, string? message = null, Exception? innerException = null)
        => throw new ArgumentException(message, paramName, innerException);

    [DoesNotReturn]
    public static void ThrowArgumentException(string? paramName = null, string? message = null, Exception? innerException = null)
        => ThrowArgumentException<object>(message, paramName, innerException);

    [DoesNotReturn]
    public static T ThrowInvalidOperationException<T>(string? message = null, Exception? innerException = null)
        => throw new InvalidOperationException(message, innerException);

    [DoesNotReturn]
    public static void ThrowInvalidOperationException(string? message = null, Exception? innerException = null)
        => ThrowInvalidOperationException<object>(message, innerException);

    [DoesNotReturn]
    public static T ThrowArgumentOutOfRangeException<T>(string? paramName = null, object? value = default, string? message = null)
        => throw new ArgumentOutOfRangeException(paramName, value, message);

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string? paramName = null, object? value = default, string? message = null)
        => ThrowArgumentOutOfRangeException<object>(paramName, value, message);
}
