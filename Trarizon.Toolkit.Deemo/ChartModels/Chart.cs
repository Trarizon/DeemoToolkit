using Newtonsoft.Json;
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
    private Chart(float speed, int remapMinVelocity, int remapMaxVelocity, List<Note>? notes, IEnumerable<Link.Deserializer>? links, List<SpeedLine>? speedLines) :
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
            chart = FromDeV3Chart(json);
        } catch { chart = null; }

        return chart != null;
    }

    public string ToJson(ChartVersion chartVersion = ChartVersion.Deemo) 
        => ChartSerializer.Serialize(this, chartVersion);

    private static Chart? FromDeV3Chart(string json)
    {
        DeV3ChartAdapter.Chart? chart = DeV3ChartAdapter.FromJson(json);
        if (chart == null)
            return null;

        List<Note>? notes = chart.notes?.Select(ConvertNote).ToList();
        IEnumerable<Link.Deserializer>? links = chart.links?.Select(ConvertLink);

        return new Chart(chart.speed, 10, 70, notes, links, null);

        static Note ConvertNote(DeV3ChartAdapter.Note v3Note)
            => new(v3Note.pos, v3Note.size, v3Note._time, v3Note.sounds ?? new());

        Link.Deserializer ConvertLink(DeV3ChartAdapter.Link v3Link)
            => new(v3Link.notes?.Select(nref =>
            {
                int index = nref.id - 1;
                if (chart.notes?[index].id == nref.id)
                    return notes![index];
                else
                    throw new FormatException("note ids are not in order");
            }) ?? Enumerable.Empty<Note>());
    }
}
