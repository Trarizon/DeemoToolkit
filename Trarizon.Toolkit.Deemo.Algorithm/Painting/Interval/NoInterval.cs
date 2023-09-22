using SkiaSharp;

namespace Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
internal sealed class NoInterval : IPaintingInterval
{
	public static NoInterval Instance { get; } = new();

	public IPaintingInterval Clone() => Instance;
	public IEnumerable<PaintingIntervalTag> GetTags(NotesPainter painter) => Enumerable.Empty<PaintingIntervalTag>();
	public float GetTextWidth(SKPaint paint) => 0f;
}
