namespace Trarizon.Library.CLParsing.Input;
internal sealed class EnumerableInput : IInputArguments
{
    private readonly IEnumerator<string> _source;

    public EnumerableInput(IEnumerable<string> source) 
        => _source = source.GetEnumerator();

    // input type "--prm=value" is not support in EnumerableInput
    public InputSplit Current => new(_source.Current, InputSplit.Normal);

    public bool Next() => _source.MoveNext();
}
