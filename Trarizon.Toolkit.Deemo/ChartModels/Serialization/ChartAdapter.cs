using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Trarizon.Toolkit.Deemo.ChartModels.Serialization;
internal static class ChartAdapter
{
    private static readonly JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings { ReferenceResolverProvider = () => DeserializerReferenceResolver.Instance });

    private sealed class DeserializerReferenceResolver : IReferenceResolver
    {
        public static readonly DeserializerReferenceResolver Instance = new DeserializerReferenceResolver();

        public void AddReference(object context, string reference, object value) => throw new NotImplementedException();
        public string GetReference(object context, object value) => throw new NotImplementedException();
        public bool IsReferenced(object context, object value) => throw new NotImplementedException();
        public object ResolveReference(object context, string reference) => new DeV3Link.NoteRef { id = int.Parse(reference) };
    }

    public static DeV3Chart ToDeV3Chart(Chart chart)
    {
        List<DeV3Note> notes = new(chart.Notes.Count);
        Dictionary<Note, int> linkNoteIds = new();
        List<Note> linkHeads = new();

        for (int i = 0; i < chart.Notes.Count; i++) {
            Note note = chart.Notes[i];
            int noteId = i + 1;

            // Note
            notes.Add(new DeV3Note {
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

        return new DeV3Chart {
            notes = notes,
            links = linkHeads.Select(n => new DeV3Link {
                notes = new Link(n).Notes.Select(n => new DeV3Link.NoteRef { id = linkNoteIds[n] })
            }),
            speed = chart.Speed,
        };
    }

    private static Chart ToGeneralChart(DeV3Chart chart)
    {
        List<Note>? notes = chart.notes?.Select(ConvertNote).ToList();
        IEnumerable<Link.Deserializer>? links = chart.links?.Select(ConvertLink);

        return new Chart(chart.speed, 10, 70, notes, links, null);

        static Note ConvertNote(DeV3Note v3Note)
            => new(v3Note.pos, v3Note.size, v3Note._time, v3Note.sounds ?? new());

        Link.Deserializer ConvertLink(DeV3Link v3Link)
            => new(v3Link.notes?.Select(nref =>
            {
                int index = nref.id - 1;
                if (chart.notes?[index].id == nref.id)
                    return notes![index];
                else
                    throw new FormatException("note ids are not in order");
            }) ?? Enumerable.Empty<Note>());
    }

    public static Chart? ParseFromV3Json(string json)
    {
        JObject? jobj = JsonConvert.DeserializeObject<JObject>(json);
        if (jobj == null)
            return null;

        // Cast $ref to string for deserialization
        foreach (var refToken in jobj["links"]!.SelectMany(link => link["notes"]!)) {
            refToken["$ref"] = refToken["$ref"]!.ToString();
        }

        var v3cht = jobj.ToObject<DeV3Chart>(_serializer);
        if (v3cht == null)
            return null;

        return ToGeneralChart(v3cht);
    }

    #region Types

    public class DeV3Chart
    {
        public List<DeV3Note>? notes;
        public IEnumerable<DeV3Link>? links;
        public float speed;
    }

    public struct DeV3Link
    {
        public IEnumerable<NoteRef>? notes;

        public struct NoteRef
        {
            [JsonProperty("$ref")]
            public int id;
        }
    }

    public class DeV3Note
    {
        public float _time;
        [JsonProperty("$id")]
        public int id;
        public List<PianoSound>? sounds;
        public float pos;
        public float size;
    }

    #endregion
}
