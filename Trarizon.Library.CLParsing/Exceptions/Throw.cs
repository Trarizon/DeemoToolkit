using System.Diagnostics.CodeAnalysis;
using Trarizon.Library.CLParsing.Signature.CLParameters;

namespace Trarizon.Library.CLParsing.Exceptions;
internal static class Throw
{
    [DoesNotReturn]
    public static T RegulationInitializeFailed<T>(string message = "")
        => throw new CLParsingException(message, ExceptionKind.RegulationInitializeFailed);

    [DoesNotReturn]
    public static void RegulationInitializeFailed(string message = "")
        => throw new CLParsingException(message, ExceptionKind.RegulationInitializeFailed);

    [DoesNotReturn]
    public static T ArgumentsObjectCreateFailed<T>(string message)
        => throw new CLParsingException(message, ExceptionKind.ArgumentsObjectCreateFailed);

    [DoesNotReturn]
    public static void InputParameterRepeated(OptionParameter parameter)
        => throw new CLInputException(parameter, "Repeated input.");

    [DoesNotReturn]
    public static T InputEndUnexceptedly<T>(OptionParameter parameter)
        => throw new CLInputException(parameter, "Unexcepted input end.");
}
