namespace Trarizon.Library.CLParsing.Exceptions;
public enum ExceptionKind
{
    RegulationInitializeFailed,
    ArgumentsObjectCreateFailed,
    InvalidInput,
}

public static class InternalExceptionKindExtensions
{
    public static string GetDescription(this ExceptionKind kind) => kind switch {
        ExceptionKind.RegulationInitializeFailed => "Regulation initialize failed.",
        ExceptionKind.ArgumentsObjectCreateFailed => "Aargumetns object create failed.",
        ExceptionKind.InvalidInput => "Invalid input.",
        _ => throw new NotImplementedException(),
    };
}