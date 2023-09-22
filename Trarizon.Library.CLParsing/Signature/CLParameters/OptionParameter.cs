using Trarizon.Library.CLParsing.Attributes;
using Trarizon.Library.CLParsing.Converters;
using Trarizon.Library.CLParsing.Signature.ParameterInfos;

namespace Trarizon.Library.CLParsing.Signature.CLParameters;
internal sealed class OptionParameter : ICLParameter
{
    private readonly IParameterInfo _paramInfo;
    public CLOptionAttribute Attribute { get; }

    public IParameterInfo ParameterInfo => _paramInfo;

    CLParameterAttribute ICLParameter.Attribute => Attribute;

    public OptionParameter(IParameterInfo paramInfo, CLOptionAttribute attribute)
    {
        _paramInfo = paramInfo;
        Attribute = attribute;
    }

    public object? ConvertValue(string input)
        => InputConverter.Convert(_paramInfo.ParameterType, input);
}
