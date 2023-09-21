using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Trarizon.Toolkit.Deemo.ChartModels.Serialization;
internal static class ChartSerializer
{
	private static readonly JsonSerializerSettings DeemoV1Settings = new() {
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		ContractResolver = new ContractResolver(ChartPropertyVersions.DeemoV1, JsonPropertyHandlers.IgnoreEmptySounds),
	};
	private static readonly JsonSerializerSettings DeemoV2Settings = new() {
		ContractResolver = new ContractResolver(ChartPropertyVersions.DeemoV2, JsonPropertyHandlers.IgnoreEmptySounds),
	};
	private static readonly JsonSerializerSettings DeemoV3Settings = new() {
		ContractResolver = new ContractResolver(ChartPropertyVersions.DeemoV3, JsonPropertyHandlers.IgnoreEmptySounds),
	};
	private static readonly JsonSerializerSettings DeemoRebornSettings = new() {
		ContractResolver = new ContractResolver(ChartPropertyVersions.DeemoReborn, JsonPropertyHandlers.IgnoreEmptySounds),
	};
	private static readonly JsonSerializerSettings DeemoIIV1Settings = new() {
		ContractResolver = new ContractResolver(ChartPropertyVersions.DeemoIIV1, JsonPropertyHandlers.EmptySoundsToNull),
		Formatting = Formatting.Indented,
	};
	private static readonly JsonSerializerSettings DeemoIIV2Settings = new() {
		ContractResolver = new ContractResolver(ChartPropertyVersions.DeemoIIV2, JsonPropertyHandlers.EmptySoundsToNull),
	};

	public static string Serialize(Chart chart, ChartVersion chartVersion)
	{
		JsonSerializerSettings? settings = chartVersion switch {
			ChartVersion.Deemo => DeemoV1Settings,
			ChartVersion.DeemoV2 => DeemoV2Settings,
			ChartVersion.DeemoV3 => DeemoV3Settings,
			ChartVersion.Reborn => DeemoRebornSettings,
			ChartVersion.DeemoII => DeemoIIV1Settings,
			ChartVersion.DeemoIIV2 => DeemoIIV2Settings,
			_ => throw new NotImplementedException(),
		};

		if (chartVersion == ChartVersion.DeemoV3)
			return JsonConvert.SerializeObject(DeV3ChartAdapter.ToDeV3Chart(chart), settings);
		else
			return JsonConvert.SerializeObject(chart, settings);
	}

	private class ContractResolver : DefaultContractResolver
	{
		private readonly ChartPropertyVersions _version;
		private readonly Action<JsonProperty>? _jPropHandler;

		public ContractResolver(ChartPropertyVersions version, Action<JsonProperty>? jsonPropertyHandler = null)
		{
			_version = version;
			_jPropHandler = jsonPropertyHandler;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty jProperty = base.CreateProperty(member, memberSerialization);
			ChartPropertyVersionAttribute? attr = member.GetCustomAttribute<ChartPropertyVersionAttribute>();

			if (attr?.PropertyVersion.HasFlag(_version) == false) {
				jProperty.ShouldSerialize = _ => false;
			}
			else {
				_jPropHandler?.Invoke(jProperty);
			}
			return jProperty;
		}
	}

	public static class JsonPropertyHandlers
	{
		public static readonly Action<JsonProperty> IgnoreEmptySounds = jp =>
		{
			if (jp.PropertyName == JsonPropertyNames.Sounds)
				jp.ShouldSerialize = o => o is not Note note || note.HasSound;
		};

		public static readonly Action<JsonProperty> EmptySoundsToNull = jp =>
		{
			if (jp.PropertyName == JsonPropertyNames.Sounds)
				jp.Converter = EmptyListToNullConverter<PianoSound>.Instance;
		};

		private class EmptyListToNullConverter<T> : JsonConverter
		{
			public static EmptyListToNullConverter<T> Instance = new();

			public override bool CanConvert(Type objectType) => true;
			public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
				=> existingValue;
			public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
				=> serializer.Serialize(writer, ((List<T>)value!).Count > 0 ? value : null);
		}
	}
}
