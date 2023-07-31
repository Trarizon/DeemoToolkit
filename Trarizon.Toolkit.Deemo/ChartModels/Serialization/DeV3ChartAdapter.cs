using Newtonsoft.Json;
using GeneralChart = Trarizon.Toolkit.Deemo.ChartModels.Chart;
using GeneralLink = Trarizon.Toolkit.Deemo.ChartModels.Link;
using GeneralNote = Trarizon.Toolkit.Deemo.ChartModels.Note;

namespace Trarizon.Toolkit.Deemo.ChartModels.Serialization;
internal static class DeV3ChartAdapter
{
    public static Chart? FromJson(string json)
        => JsonConvert.DeserializeObject<Chart>(json);

    public static Chart GetV3Chart(GeneralChart chart)
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
        public int id;
        public List<PianoSound>? sounds;
        public float pos;
        public float size;
    }
}
