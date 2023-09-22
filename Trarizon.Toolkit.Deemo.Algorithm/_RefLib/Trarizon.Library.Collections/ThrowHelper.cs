using System.Diagnostics.CodeAnalysis;

namespace Trarizon.Library.Collections;
internal static class ThrowHelper
{
#if NET6_0_OR_GREATER
    [DoesNotReturn]
#endif
	public static void ThrowInvalidOperation(string? message = null)
		=> throw new InvalidOperationException(message);

#if NET6_0_OR_GREATER
    [DoesNotReturn]
#endif
	public static void ThrowArgumentOutOfRange(string? paramName, string? message = null)
		=> throw new ArgumentOutOfRangeException(paramName, message);

#if NET6_0_OR_GREATER
    [DoesNotReturn]
#endif
	public static void ThrowNotSupport(string? message = null)
		=> throw new NotSupportedException(message);

#if NET6_0_OR_GREATER
    [DoesNotReturn]
#endif
	public static T ThrowNotSupport<T>(string? message = null)
		=> throw new NotSupportedException(message);

#if NETSTANDARD2_0
	public static void ThrowArgumentNull(string? paramName, string? message = null)
		=> throw new ArgumentNullException(paramName, message);
#endif
}
