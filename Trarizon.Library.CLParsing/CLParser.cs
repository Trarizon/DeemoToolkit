using Trarizon.Library.CLParsing.Input;
using Trarizon.Library.CLParsing.Signature;

namespace Trarizon.Library.CLParsing;
public class CLParser
{
    internal const char PairSeperator = '=';
    internal const char Quote = '"';

    public static CLParser Default { get; } = new(CLSettings.Default);

    internal readonly string _fullNamePrefix;
    internal readonly string _shortNamePrefix;

    public CLParser(CLSettings settings)
    {
        _fullNamePrefix = settings.FullNamePrefix;
        _shortNamePrefix = settings.ShortNamePrefix;
    }

    public T Parse<T>(string input) => Parse<T>(input.AsMemory());
    public T Parse<T>(ReadOnlyMemory<char> input) => Parse<T>(new StringInput(input));
    public T Parse<T>(IEnumerable<string> input) => Parse<T>(new EnumerableInput(input));

    public T Parse<T>(params string[] input) => Parse<T>(input.AsEnumerable());

    private T Parse<T>(IInputArguments inputs)
    {
        Regulation regulation = Regulation.Get<T>();
        RawArguments arguments = RawArguments.Parse(inputs, regulation, this);

        return (T)regulation.Construct(arguments);
    }
}
