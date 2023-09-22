using SkiaSharp;

namespace Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
public sealed class TimeInterval : IPaintingInterval
{
	public float _time;

	public TimeInterval(float time) => _time = time;

	public IPaintingInterval Clone() => new TimeInterval(_time);

	public IEnumerable<PaintingIntervalTag> GetTags(NotesPainter painter)
	{
		int multiple = 1;
		float maxTime = painter.MaxTime;
		while (true) {
			float value = _time * multiple++;
			if (value <= maxTime)
				yield return new PaintingIntervalTag(value, value.ToString("F2"));
			else
				yield break;
		}
	}

	public float GetTextWidth(SKPaint paint) => paint.MeasureText("300.00");
}
