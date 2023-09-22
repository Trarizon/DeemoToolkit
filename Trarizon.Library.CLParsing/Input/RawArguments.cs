using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Trarizon.Library.CLParsing.Exceptions;
using Trarizon.Library.CLParsing.Signature;
using Trarizon.Library.CLParsing.Signature.CLParameters;
using Trarizon.Library.CLParsing.Utility;

namespace Trarizon.Library.CLParsing.Input;
internal sealed class RawArguments
{
    private readonly Dictionary<string, string> _options;
    private readonly HashSet<string> _flags;
    private readonly List<string> _values;

    private RawArguments()
    {
        _options = new();
        _flags = new();
        _values = new();
    }

    public static RawArguments Parse(IInputArguments input, Regulation regulation, CLParser parser)
    {
        RawArguments result = new();

        while (input.Next()) {
            InputSplit split = input.Current;

            if (!HandleOptionalParameter(split, parser._fullNamePrefix, regulation.OptionParametersByFullName)
                && !HandleOptionalParameter(split, parser._shortNamePrefix, regulation.OptionParametersByShortName)) {
                result.AddValue(split.Value.AsString());
            }
        }

        return result;

        bool HandleOptionalParameter(InputSplit split, string prefix, IReadOnlyDictionary<string, OptionParameter> parameters)
        {
            ReadOnlySpan<char> arg = split.Value;
            if (!arg.StartsWith(prefix))
                return false;

            if (split.SeperatorIndex < 0) {
                string key = arg[prefix.Length..].AsString();
                // As value
                if (!parameters.TryGetValue(key, out var prm))
                    result.AddValue(arg.AsString());
                // As parameter
                else if (prm.ParameterInfo.ParameterType == typeof(bool))
                    result.AddFlag(prm);
                else if (input.Next())
                    result.AddOption(prm, input.Current.Value.AsString());
                else
                    return Throw.InputEndUnexceptedly<bool>(prm);
            }
            else {
                string key = arg[prefix.Length..split.SeperatorIndex].AsString();
                // As value
                if (!parameters.TryGetValue(key, out var prm))
                    result.AddValue(arg.AsString());
                // As parameter
                else {
                    ReadOnlySpan<char> value = arg[(split.SeperatorIndex + 1)..];
                    if (value[0] == CLParser.Quote)
                        value = value[1..^1].Unescape();
                    result.AddOption(prm, value.AsString());
                }
            }
            return true;
        }
    }

    private void AddFlag(OptionParameter parameter)
    {
        if (_flags.Contains(parameter.Attribute.FullName))
            Throw.InputParameterRepeated(parameter);
        _flags.Add(parameter.Attribute.FullName);
    }

    private void AddOption(OptionParameter parameter, string value)
    {
        if (!_options.TryAdd(parameter.Attribute.FullName, value))
            Throw.InputParameterRepeated(parameter);
    }

    private void AddValue(string value)
    {
        _values.Add(value);
    }

    public bool GetOptionValue(string key, [NotNullWhen(true)] out string? value)
        => _options.TryGetValue(key, out value);

    public bool HasFlag(string flag) => _flags.Contains(flag);

    public Values GetValues() => new(this);

    public ref struct Values
    {
        private readonly ReadOnlySpan<string> _values;
        private int _index;

        public readonly bool IsEnd => _index >= _values.Length;

        internal Values(RawArguments rawArguments)
        {
            _values = CollectionsMarshal.AsSpan(rawArguments._values);
            _index = 0;
        }

        public ReadOnlySpan<string> Read(int exceptedCount)
        {
            if (IsEnd)
                return ReadOnlySpan<string>.Empty;

            int start = _index;
            int actualCount = int.Min(_values.Length - _index, exceptedCount);
            _index += actualCount;
            return _values[start.._index];
        }

        public bool Read([NotNullWhen(true)] out string? value)
        {
            if (IsEnd) {
                value = null;
                return false;
            }

            value = _values[_index++];
            return true;
        }

        public bool Next() => ++_index < _values.Length;
    }
}
