using Trarizon.Library.CLParsing.Attributes;
using Trarizon.Library.CLParsing.Converters;
using Trarizon.Library.CLParsing.Input;
using Trarizon.Library.CLParsing.Signature.ParameterInfos;

namespace Trarizon.Library.CLParsing.Signature.CLParameters;
internal sealed class ValueParameter : ICLParameter
{
    private readonly IParameterInfo _paramInfo;

    public CLValueAttribute Attribute { get; }

    public IParameterInfo ParameterInfo => _paramInfo;

    CLParameterAttribute ICLParameter.Attribute => Attribute;

    public ValueParameter(IParameterInfo paramInfo, CLValueAttribute attribute)
    {
        _paramInfo = paramInfo;
        Attribute = attribute;
    }

    public object? ConvertValue(ref RawArguments.Values inputValues)
    {
        object? value = null;
        Type type = _paramInfo.ParameterType;

        // Array
        if (type.IsArray) {
            value = InputConverter.ConvertToArray(type.GetElementType()!, inputValues.Read(Attribute.MaxCount));
        }
        // Generic collections
        else if (type.IsGenericType) {
            Type genericTypeDefination = type.GetGenericTypeDefinition();
            if (genericTypeDefination == typeof(IEnumerable<>)
                || genericTypeDefination == typeof(IReadOnlyList<>))
                value = InputConverter.ConvertToArray(type.GenericTypeArguments[0], inputValues.Read(Attribute.MaxCount));
            else if (genericTypeDefination == typeof(List<>)
                || genericTypeDefination == typeof(IList<>))
                value = InputConverter.ConvertToList(type.GenericTypeArguments[0], inputValues.Read(Attribute.MaxCount));
        }

        // Single Argument
        if (value == null && inputValues.Read(out var text)) {
            value = InputConverter.Convert(type, text);
        }

        // else default
        return value;
    }
}
