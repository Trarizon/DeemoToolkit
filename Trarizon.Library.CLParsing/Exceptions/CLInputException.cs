using Trarizon.Library.CLParsing.Signature.CLParameters;

namespace Trarizon.Library.CLParsing.Exceptions;
public class CLInputException : CLParsingException
{
    internal CLInputException(OptionParameter parameter, string message) :
        base($"OptionParameter <{parameter.Attribute.FullName}|{parameter.Attribute.FullName}>: {message}",
        ExceptionKind.InvalidInput)
    { }
}
