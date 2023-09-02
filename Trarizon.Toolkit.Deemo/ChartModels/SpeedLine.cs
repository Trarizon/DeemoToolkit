using Newtonsoft.Json;
using System.ComponentModel;
using Trarizon.Toolkit.Deemo.ChartModels.Serialization;

namespace Trarizon.Toolkit.Deemo.ChartModels;
[JsonObject(MemberSerialization.OptIn)]
public sealed class SpeedLine
{
    [JsonProperty(JsonPropertyNames.Speed)]
    public float Speed { get; set; }

    [JsonProperty(JsonPropertyNames.StartTime)]
    public float StartTime { get; set; }

    [JsonProperty(JsonPropertyNames.EndTime)]
    public float EndTime { get; set; }

    [JsonProperty(JsonPropertyNames.WarningType)]
    [DefaultValue(WarningType.Default)]
    public WarningType WarningType { get; set; }

    [JsonConstructor]
    public SpeedLine(float speed, float startTime, float endTime, WarningType warningType = WarningType.Default)
    {
        Speed = speed;
        StartTime = startTime;
        EndTime = endTime;
        WarningType = warningType;
    }

    public SpeedLine(SpeedLine other) :
        this(other.Speed, other.StartTime, other.EndTime, other.WarningType)
    { }
}
