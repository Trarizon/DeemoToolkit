using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using Trarizon.Toolkit.Deemo.ChartModels.Serialization;

namespace Trarizon.Toolkit.Deemo.ChartModels;
[JsonObject(MemberSerialization.OptIn)]
public sealed class Chart
{
	#region Properties

	[JsonProperty(JsonPropertyNames.Speed)]
	public float Speed { get; set; }

	// Questionable
	[ChartPropertyVersion(ChartPropertyVersions.DeemoII)]
	[JsonProperty(JsonPropertyNames.OriVMin)]
	public int MinVelocity => Notes.SelectMany(n => n.Sounds).Min(s => s.Velocity);

	// Questionable
	[ChartPropertyVersion(ChartPropertyVersions.DeemoII)]
	[JsonProperty(JsonPropertyNames.OriVMax)]
	public int MaxVelocity => Notes.SelectMany(n => n.Sounds).Max(s => s.Velocity);

	[ChartPropertyVersion(ChartPropertyVersions.DeemoII)]
	[JsonProperty(JsonPropertyNames.RemapVMin)]
	public int RemapMinVelocity { get; set; }

	[ChartPropertyVersion(ChartPropertyVersions.DeemoII)]
	[JsonProperty(JsonPropertyNames.RemapVMax)]
	public int RemapMaxVelocity { get; set; }

	[JsonProperty(JsonPropertyNames.Notes)]
	public List<Note> Notes { get; }

	[JsonProperty(JsonPropertyNames.Links)]
	public IEnumerable<Link> Links => from n in Notes
									  where n.IsLinkHead()
									  select new Link(n);

	[ChartPropertyVersion(ChartPropertyVersions.DeemoII)]
	[JsonProperty(JsonPropertyNames.Lines)]
	public List<SpeedLine> SpeedLines { get; }

	#endregion

	#region Constructors

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

	#endregion

	#region Factories

	public static bool TryParseFromJson(string json, [NotNullWhen(true)] out Chart? chart)
	{
		try {
			chart = FromJson(json);
			return true;
		} catch {
			chart = null;
			return false;
		}
	}

	public static Chart FromJson(string json)
	{
		try {
			return JsonConvert.DeserializeObject<Chart>(json) ?? throw new FormatException("Chart is null");
		} catch {
			return ChartAdapter.ParseFromV3Json(json) ?? throw new FormatException("Chart is null");
		}
	}

	#endregion

	public string ToJson(ChartVersion chartVersion = ChartVersion.Deemo)
		=> ChartSerializer.Serialize(this, chartVersion);
}
