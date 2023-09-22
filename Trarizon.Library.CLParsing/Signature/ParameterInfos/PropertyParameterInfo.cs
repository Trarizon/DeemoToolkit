using System.Reflection;
using Trarizon.Library.CLParsing.Attributes;

namespace Trarizon.Library.CLParsing.Signature.ParameterInfos;
internal sealed class PropertyParameterInfo : IParameterPropertyInfo
{
    private readonly PropertyInfo _info;

    public Type ParameterType => _info.PropertyType;

    public string MemberName => _info.Name;

    public PropertyParameterInfo(PropertyInfo info) => _info = info;

    public void SetValue(object? obj, object? value) => _info.SetValue(obj, value);
    public IEnumerable<CLParameterAttribute> GetAttributes() => _info.GetCustomAttributes<CLParameterAttribute>();
}
