using System;
using System.Diagnostics.CodeAnalysis;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;
internal static class ThrowHelper
{
    [DoesNotReturn]
    public static T ThrowInvalidOperationException<T>(string? message = null)
        => throw new InvalidOperationException(message);

    [DoesNotReturn]
    public static void ThrowInvalidOperationException(string? message = null)
        => ThrowInvalidOperationException<object>(message);
}
