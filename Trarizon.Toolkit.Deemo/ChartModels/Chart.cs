using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using Trarizon.Toolkit.Deemo.ChartModels.Serialization;

namespace Trarizon.Toolkit.Deemo.ChartModels;
[JsonObject(MemberSerialization.OptIn)]
public sealed class Chart
{
    [JsonProperty("speed")]
    public float Speed { get; set; }

    // Questionable
    [ChartPropertyVersion(ChartPropertyVersion.DeemoII)]
    [JsonProperty("oriVMin")]
    public int MinVelocity => Notes.SelectMany(n => n.Sounds).Min(s => s.Velocity);

    // Questionable
    [ChartPropertyVersion(ChartPropertyVersion.DeemoII)]
    [JsonProperty("oriVMax")]
    public int MaxVelocity => Notes.SelectMany(n => n.Sounds).Max(s => s.Velocity);

    [ChartPropertyVersion(ChartPropertyVersion.DeemoII)]
    [JsonProperty("remapVMin")]
    public int RemapMinVelocity { get; set; }

    [ChartPropertyVersion(ChartPropertyVersion.DeemoII)]
    [JsonProperty("remapVMax")]
    public int RemapMaxVelocity { get; set; }

    [JsonProperty("notes")]
    public List<Note> Notes { get; }

    [JsonProperty("links")]
    public IEnumerable<Link> Links => from n in Notes
                                      where n.IsLinkHead()
                                      select new Link(n);

    [ChartPropertyVersion(ChartPropertyVersion.DeemoII)]
    [JsonProperty("lines")]
    public List<SpeedLine> SpeedLines { get; }

    // All property constructor
    private Chart(float speed, int remapMinVelocity, int remapMaxVelocity, List<Note> notes, List<SpeedLine> speedLines)
    {
        Speed = speed;
        RemapMinVelocity = remapMinVelocity;
        RemapMaxVelocity = remapMaxVelocity;
        Notes = notes;
        SpeedLines = speedLines;
    }

    public Chart(IEnumerable<Note>? notes = null, IEnumerable<SpeedLine>? speedLines = null, float speed = 6f, int remapMinVelocity = 10, int remapMaxVelocity = 70) :
        this(speed, remapMinVelocity, remapMaxVelocity, notes is null ? new() : new(notes), speedLines is null ? new() : new(speedLines))
    { }

    [JsonConstructor]
    internal Chart(float speed, int remapMinVelocity, int remapMaxVelocity, List<Note>? notes, IEnumerable<Link.Deserializer>? links, List<SpeedLine>? speedLines) :
        this(speed, remapMinVelocity, remapMaxVelocity, notes ?? new(), speedLines ?? new())
    {
        if (links is null) return;

        foreach (Link.Deserializer link in links) {
            Note? prev = null;
            foreach (Note note in link.Notes) {
                note.IsSlide = true;
                note.PrevLink = prev;
                if (prev != null)
                    prev.NextLink = note;
                prev = note;
            }
        }
    }

    public Chart(Chart other, bool cloneNotes = true) :
        this(other.Speed, other.RemapMinVelocity, other.RemapMaxVelocity,
            cloneNotes ? other.Notes.Select(n => new Note(n)).ToList() : new(),
            cloneNotes ? other.SpeedLines.Select(l => new SpeedLine(l)).ToList() : new())
    { }

    public static bool TryParseFromJson(string json, [NotNullWhen(true)] out Chart? chart)
    {
        try {
            chart = JsonConvert.DeserializeObject<Chart>(json);
        } catch { chart = null; }

        if (chart != null)
            return true;

        try {
            chart = DeV3ChartAdapter.ParseFromV3Json(json);
        } catch { chart = null; }

        return chart != null;
    }

    public string ToJson(ChartVersion chartVersion = ChartVersion.Deemo)
        => ChartSerializer.Serialize(this, chartVersion);
}
