namespace Trarizon.Library.CLParsing.Exceptions;
public class CLParsingException : Exception
{
    public ExceptionKind Kind { get; }

    internal CLParsingException(string message, ExceptionKind kind) :
        base(message)
        => Kind = kind;
}
