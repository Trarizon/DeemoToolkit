using SkiaSharp;

namespace Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
public sealed class ComboInterval : IPaintingInterval
{
	private readonly int _combo;

	public ComboInterval(int combo) => _combo = combo;

	public IPaintingInterval Clone() => new ComboInterval(_combo);

	public IEnumerable<PaintingIntervalTag> GetTags(NotesPainter painter)
	{
		int currentCombo = 1;
		foreach (var note in painter.Notes) {
			if (!note.IsVisible)
				continue;

			if (currentCombo % _combo == 0)
				yield return new(note.Time, currentCombo.ToString());
			currentCombo++;
		}
	}

	public float GetTextWidth(SKPaint paint) => paint.MeasureText("8776");
}
