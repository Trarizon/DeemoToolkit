using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Trarizon.Toolkit.Deemo.ChartModels.Serialization;
internal static class ChartSerializer
{
    private static readonly JsonSerializerSettings DeemoV1Settings = new() {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ContractResolver = new ContractResolver(ChartPropertyVersion.DeemoV1),
    };
    private static readonly JsonSerializerSettings DeemoV2Settings = new() {
        ContractResolver = new ContractResolver(ChartPropertyVersion.DeemoV2)
    };
    private static readonly JsonSerializerSettings DeemoRebornSettings = new() {
        ContractResolver = new ContractResolver(ChartPropertyVersion.DeemoReborn)
    };
    private static readonly JsonSerializerSettings DeemoIIV1Settings = new() {
        ContractResolver = new ContractResolver(ChartPropertyVersion.DeemoIIV1),
        Formatting = Formatting.Indented,
    };
    private static readonly JsonSerializerSettings DeemoIIV2Settings = new() {
        ContractResolver = new ContractResolver(ChartPropertyVersion.DeemoIIV2)
    };

    public static string Serialize(Chart chart, ChartVersion chartVersion)
    {
        JsonSerializerSettings? settings = chartVersion switch {
            ChartVersion.Deemo => DeemoV1Settings,
            ChartVersion.DeemoV2 => DeemoV2Settings,
            ChartVersion.DeemoV3 => null,
            ChartVersion.Reborn => DeemoRebornSettings,
            ChartVersion.DeemoII => DeemoIIV1Settings,
            ChartVersion.DeemoIIV2 => DeemoIIV2Settings,
            _ => throw new NotImplementedException(),
        };
        if (settings != null)
            return JsonConvert.SerializeObject(chart, settings);

        // V3Chart
        return JsonConvert.SerializeObject(DeV3ChartAdapter.ToDeV3Chart(chart));
    }

    private class ContractResolver : DefaultContractResolver
    {
        private readonly ChartPropertyVersion _version;

        public ContractResolver(ChartPropertyVersion version) => _version = version;

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jProperty = base.CreateProperty(member, memberSerialization);
            ChartPropertyVersionAttribute? attr = member.GetCustomAttribute<ChartPropertyVersionAttribute>();

            if (attr?.PropertyVersion.HasFlag(_version) == false)
                jProperty.ShouldSerialize = _ => false;

            return jProperty;
        }
    }

}
