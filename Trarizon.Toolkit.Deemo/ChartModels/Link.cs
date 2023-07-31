using Newtonsoft.Json;

namespace Trarizon.Toolkit.Deemo.ChartModels;
[JsonObject(MemberSerialization.OptIn)]
public readonly struct Link
{
    public readonly Note _head;

    [JsonProperty("notes")]
    public IEnumerable<Note> Notes
    {
        get {
            Note? current = _head;
            while (current != null) {
                yield return current;
                current = current.NextLink;
            }
        }
    }

    public Link(Note linkHead) => _head = linkHead;

    internal readonly struct Deserializer
    {
        public readonly IEnumerable<Note> Notes;

        [JsonConstructor]
        public Deserializer(IEnumerable<Note> notes) => Notes = notes;
    }
}
