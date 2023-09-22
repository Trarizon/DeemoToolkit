using Trarizon.Library.CLParsing.Attributes;
using Trarizon.Library.CLParsing.Signature.ParameterInfos;

namespace Trarizon.Library.CLParsing.Signature.CLParameters;
internal interface ICLParameter
{
    CLParameterAttribute Attribute { get; }
    IParameterInfo ParameterInfo { get; }
}
