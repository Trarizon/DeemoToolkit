namespace Trarizon.Library.CLParsing.Input;
internal interface IInputArguments
{
    bool Next();
    InputSplit Current { get; }
}
