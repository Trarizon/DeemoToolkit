using Trarizon.Library.CLParsing.Attributes;

namespace Trarizon.Library.CLParsing.Signature.ParameterInfos;
internal interface IParameterInfo
{
    string MemberName { get; }
    Type ParameterType { get; }

    IEnumerable<CLParameterAttribute> GetAttributes();
}
