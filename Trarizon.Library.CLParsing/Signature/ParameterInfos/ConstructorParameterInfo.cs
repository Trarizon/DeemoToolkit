using System.Reflection;
using Trarizon.Library.CLParsing.Attributes;

namespace Trarizon.Library.CLParsing.Signature.ParameterInfos;
internal sealed class ConstructorParameterInfo : IParameterInfo
{
    private readonly ParameterInfo _info;

    public string MemberName => _info.Name!;

    public Type ParameterType => _info.ParameterType;

    public ConstructorParameterInfo(ParameterInfo info)
    {
        _info = info;
    }

    public IEnumerable<CLParameterAttribute> GetAttributes() => _info.GetCustomAttributes<CLParameterAttribute>();
}
