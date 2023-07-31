using SkiaSharp;

namespace Trarizon.Toolkit.Deemo.Commands.Components.Painting.Interval;
internal sealed class NoInterval : IPaintingInterval
{
    public static NoInterval Instance { get; } = new();

    public IPaintingInterval Clone() => Instance;
    public IEnumerable<PaintingIntervalTag> GetTags(ChartPainter painter) => Enumerable.Empty<PaintingIntervalTag>();
    public float GetTextWidth(SKPaint paint) => 0f;
}
