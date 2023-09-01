using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using GeneralChart = Trarizon.Toolkit.Deemo.ChartModels.Chart;
using GeneralLink = Trarizon.Toolkit.Deemo.ChartModels.Link;
using GeneralNote = Trarizon.Toolkit.Deemo.ChartModels.Note;

namespace Trarizon.Toolkit.Deemo.ChartModels.Serialization;
internal static class DeV3ChartAdapter
{
    private static JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings { ReferenceResolverProvider = () => DeserializerReferenceResolver.Instance });

    private sealed class DeserializerReferenceResolver : IReferenceResolver
    {
        public static readonly DeserializerReferenceResolver Instance = new DeserializerReferenceResolver();

        public void AddReference(object context, string reference, object value) => throw new NotImplementedException();
        public string GetReference(object context, object value) => throw new NotImplementedException();
        public bool IsReferenced(object context, object value) => throw new NotImplementedException();
        public object ResolveReference(object context, string reference) => new Link.NoteRef { id = int.Parse(reference) };
    }

    public static Chart ToDeV3Chart(GeneralChart chart)
    {
        List<Note> notes = new(chart.Notes.Count);
        Dictionary<GeneralNote, int> linkNoteIds = new();
        List<GeneralNote> linkHeads = new();

        for (int i = 0; i < chart.Notes.Count; i++) {
            GeneralNote note = chart.Notes[i];
            int noteId = i + 1;

            // Note
            notes.Add(new Note {
                _time = note.Time,
                id = noteId,
                sounds = note.Sounds,
                pos = note.Position,
                size = note.Size,
            });

            // Link
            if (note.IsSlide) {
                linkNoteIds.Add(note, noteId);
                if (note.PrevLink == null)
                    linkHeads.Add(note);
            }
        }

        return new Chart {
            notes = notes,
            links = linkHeads.Select(n => new Link {
                notes = new GeneralLink(n).Notes.Select(n => new Link.NoteRef { id = linkNoteIds[n] })
            }),
            speed = chart.Speed,
        };
    }

    public static GeneralChart ToGeneralChart(Chart chart)
    {
        List<GeneralNote>? notes = chart.notes?.Select(ConvertNote).ToList();
        IEnumerable<GeneralLink.Deserializer>? links = chart.links?.Select(ConvertLink);

        return new GeneralChart(chart.speed, 10, 70, notes, links, null);

        static GeneralNote ConvertNote(Note v3Note)
            => new(v3Note.pos, v3Note.size, v3Note._time, v3Note.sounds ?? new());

        GeneralLink.Deserializer ConvertLink(Link v3Link)
            => new(v3Link.notes?.Select(nref =>
            {
                int index = nref.id - 1;
                if (chart.notes?[index].id == nref.id)
                    return notes![index];
                else
                    throw new FormatException("note ids are not in order");
            }) ?? Enumerable.Empty<GeneralNote>());
    }

    public static GeneralChart? ParseFromV3Json(string json)
    {
        JObject? jobj = JsonConvert.DeserializeObject<JObject>(json);
        if (jobj == null)
            return null;

        // Cast $ref to string for deserialization
        foreach (var refToken in jobj["links"]!.SelectMany(link => link["notes"]!)) {
            refToken["$ref"] = refToken["$ref"]!.ToString();
        }

        var v3cht = jobj.ToObject<Chart>(_serializer);
        if (v3cht == null)
            return null;

        return ToGeneralChart(v3cht);
    }

    public class Chart
    {
        public List<Note>? notes;
        public IEnumerable<Link>? links;
        public float speed;
    }

    public struct Link
    {
        public IEnumerable<NoteRef>? notes;

        public struct NoteRef
        {
            [JsonProperty("$ref")]
            public int id;
        }
    }

    public class Note
    {
        public float _time;
        [JsonProperty("$id")]
        public int id;
        public List<PianoSound>? sounds;
        public float pos;
        public float size;
    }
}
