using Newtonsoft.Json;
using System.ComponentModel;

namespace Trarizon.Toolkit.Deemo.ChartModels;
[JsonObject(MemberSerialization.OptIn)]
public sealed class SpeedLine
{
    [JsonProperty("speed")]
    public float Speed { get; set; }

    [JsonProperty("startTime")]
    public float StartTime { get; set; }

    [JsonProperty("endTime")]
    public float EndTime { get; set; }

    [JsonProperty("warningType")]
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
