using Newtonsoft.Json;
using System.ComponentModel;
using Trarizon.Toolkit.Deemo.ChartModels.Serialization;

#pragma warning disable CS0618 // 类型或成员已过时

namespace Trarizon.Toolkit.Deemo.ChartModels;
[JsonObject(MemberSerialization.OptIn, IsReference = true)]
public sealed class Note
{
    #region Serializable Properties

    [Obsolete("Suspected redundant field, use IsLink to instead")]
    [ChartPropertyVersion(ChartPropertyVersions.DeemoV2 | ChartPropertyVersions.DeemoReborn)]
    [JsonProperty(JsonPropertyNames.Type)]
    public NoteType Type { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.All)]
    [JsonProperty(JsonPropertyNames.Sounds)]
    public List<PianoSound> Sounds { get; private set; }

    [ChartPropertyVersion(ChartPropertyVersions.All)]
    [JsonProperty(JsonPropertyNames.Pos)]
    public float Position { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.All)]
    [JsonProperty(JsonPropertyNames.Size)]
    public float Size { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.All)]
    [JsonProperty(JsonPropertyNames.UnderlineTime)]
    public float Time { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.DeemoV2 | ChartPropertyVersions.DeemoReborn | ChartPropertyVersions.DeemoII)]
    [JsonProperty(JsonPropertyNames.Shift)]
    public float Shift { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.DeemoII)]
    [JsonProperty(JsonPropertyNames.Speed)]
    public float Speed { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.DeemoII)]
    [JsonProperty(JsonPropertyNames.Duration)]
    public float Duration { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.DeemoReborn | ChartPropertyVersions.DeemoII)]
    [JsonProperty(JsonPropertyNames.Vibrate)]
    public bool Vibrate { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.DeemoIIV2)]
    [JsonProperty(JsonPropertyNames.Swipe)]
    public bool IsSwipe { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.DeemoReborn | ChartPropertyVersions.DeemoII)]
    [JsonProperty(JsonPropertyNames.WarningType)]
    public WarningType WarningType { get; set; }

    [ChartPropertyVersion(ChartPropertyVersions.DeemoReborn | ChartPropertyVersions.DeemoII)]
    [JsonProperty(JsonPropertyNames.EventId)]
    [DefaultValue("")]
    public string EventId { get; set; }

    [Obsolete("Supsected redundant field, the value will always be same as Time")]
    [ChartPropertyVersion(ChartPropertyVersions.DeemoV2 | ChartPropertyVersions.DeemoReborn | ChartPropertyVersions.DeemoII)]
    [JsonProperty(JsonPropertyNames.Time)]
    public float AnotherTime { get => Time; set { } }

    #endregion

    #region Properties

    private bool _isSlide;
    private Note? _prevLink;
    private Note? _nextLink;

    public bool IsSlide
    {
        get => _isSlide;
        set {
            if (!value)
                _prevLink = _nextLink = null;
            _isSlide = value;
        }
    }

    public Note? PrevLink
    {
        get => _prevLink;
        set {
            if (value != null)
                _isSlide = true;
            _prevLink = value;
        }
    }

    public Note? NextLink
    {
        get => _nextLink;
        set {
            if (value != null)
                _isSlide = true;
            _nextLink = value;
        }
    }

    public bool HasSound => Sounds.Count > 0;

    public bool IsVisible => Position is <= 2f and >= -2f;

    public bool IsHold => !IsSwipe && Duration > 0f;

    public float EndTime => Time + Duration;

    #endregion

    #region Constructors

    // All property constructor
    [JsonConstructor]
    private Note(List<PianoSound>? sounds, float position, float size, float time,
        float speed, float duration, bool vibrate, bool isSwipe, WarningType warningType, string eventId,
        NoteType type, float anotherTime)
    {
        Sounds = sounds ?? new List<PianoSound>();
        Position = position;
        Size = size;
        Time = time;
        Speed = speed;
        Duration = duration;
        Vibrate = vibrate;
        IsSwipe = isSwipe;
        WarningType = warningType;
        EventId = eventId;
        Type = type;
        AnotherTime = anotherTime;
    }

    /// <summary>
    /// Create a Note, this method assign <paramref name="sounds"/> to <see cref="Sounds"/>,
    /// which means any change you made to <paramref name="sounds"/> will affect <see cref="Sounds"/>
    /// </summary>
    public Note(float position, float size, float time, List<PianoSound> sounds,
        float speed = 1f, float duration = 0f, bool isSwipe = false, WarningType warningType = WarningType.Default) :
        this(sounds, position, size, time, speed, duration, false, isSwipe, warningType, "", NoteType.Hit, time)
    { }

    /// <summary>
    /// Create a Note, this constructor will copy contents of <paramref name="sounds"/>
    /// to <see cref="Sounds"/>
    /// </summary>
    public Note(float position, float size, float time, IEnumerable<PianoSound>? sounds = null,
        float speed = 1f, float duration = 0f, bool isSwipe = false, WarningType warningType = WarningType.Default) :
        this(position, size, time, sounds is null ? new() : new(sounds), speed, duration, isSwipe, warningType)
    { }

    public Note(Note other, bool cloneSounds = true) :
        this(cloneSounds ? other.Sounds.Select(s => new PianoSound(s)).ToList() : new(),
            other.Position, other.Size, other.Time, other.Speed, other.Duration, other.Vibrate, other.IsSwipe, other.WarningType, other.EventId, other.Type, other.AnotherTime)
    { }

	#endregion

	[Obsolete("Suspected redundant field, use IsLink to instead")]
	public enum NoteType
    {
        Hit,
        Slide,
    }
}
