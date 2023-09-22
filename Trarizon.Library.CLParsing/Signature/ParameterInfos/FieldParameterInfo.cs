using System.Reflection;
using Trarizon.Library.CLParsing.Attributes;

namespace Trarizon.Library.CLParsing.Signature.ParameterInfos;
internal sealed class FieldParameterInfo : IParameterPropertyInfo
{
    private readonly FieldInfo _info;

    public Type ParameterType => _info.FieldType;

    public string MemberName => _info.Name;

    public FieldParameterInfo(FieldInfo info)
    {
        _info = info;
    }

    public void SetValue(object obj, object? value)
         => _info.SetValue(obj, value);

    public IEnumerable<CLParameterAttribute> GetAttributes() => _info.GetCustomAttributes<CLParameterAttribute>();
}
