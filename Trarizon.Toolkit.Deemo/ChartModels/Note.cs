using Newtonsoft.Json;
using System.ComponentModel;
using Trarizon.Toolkit.Deemo.ChartModels.Serialization;

namespace Trarizon.Toolkit.Deemo.ChartModels;
[JsonObject(MemberSerialization.OptIn, IsReference = true)]
public sealed class Note
{
    [Obsolete("Suspected redundant field")]
    [ChartPropertyVersion(ChartPropertyVersion.DeemoV2 | ChartPropertyVersion.DeemoReborn)]
    [JsonProperty("type")]
    public NoteType Type { get; set; }

    [ChartPropertyVersion(ChartPropertyVersion.All)]
    [JsonProperty("sounds")]
    private List<PianoSound>? _sounds;
    public List<PianoSound> Sounds => _sounds ??= new List<PianoSound>();

    [ChartPropertyVersion(ChartPropertyVersion.All)]
    [JsonProperty("pos")]
    public float Position { get; set; }

    [ChartPropertyVersion(ChartPropertyVersion.All)]
    [JsonProperty("size")]
    public float Size { get; set; }

    [ChartPropertyVersion(ChartPropertyVersion.All)]
    [JsonProperty("_time")]
    public float Time { get; set; }

    [ChartPropertyVersion(ChartPropertyVersion.DeemoV2 | ChartPropertyVersion.DeemoReborn | ChartPropertyVersion.DeemoII)]
    [JsonProperty("shift")]
    public float Shift { get; set; }

    [ChartPropertyVersion(ChartPropertyVersion.DeemoII)]
    [JsonProperty("speed")]
    [DefaultValue(1f)]
    public float Speed { get; set; }

    [ChartPropertyVersion(ChartPropertyVersion.DeemoII)]
    [JsonProperty("duration")]
    public float Duration { get; set; }

    [ChartPropertyVersion(ChartPropertyVersion.DeemoII)]
    [JsonProperty("vibrate")]
    public bool Vibrate { get; set; }

    [JsonProperty("swipe")]
    public bool IsSwipe { get; set; }

    [JsonProperty("warningType")]
    public WarningType WarningType { get; set; }

    [JsonProperty("eventID")]
    [DefaultValue("")]
    public string EventId { get; set; }

    [Obsolete("Supsected redundant field, the value will always be same as Time")]
    [JsonProperty("time")]
    public float AnotherTime { get => Time; set { } }

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

    // All property constructor
    [JsonConstructor]
    private Note(List<PianoSound> sounds, float position, float size, float time,
        float speed, float duration, bool vibrate, bool isSwipe, WarningType warningType, string eventId,
        NoteType type, float anotherTime)
    {
#pragma warning disable CS0618
        _sounds = sounds;
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
#pragma warning restore CS0618
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
#pragma warning disable CS0618 // 类型或成员已过时
        this(cloneSounds ? other.Sounds.Select(s => new PianoSound(s)).ToList() : new(),
            other.Position, other.Size, other.Time, other.Speed, other.Duration, other.Vibrate, other.IsSwipe, other.WarningType, other.EventId, other.Type, other.AnotherTime)
#pragma warning restore CS0618 // 类型或成员已过时
    { }

    public enum NoteType
    {
        Hit,
        Slide,
    }
}
