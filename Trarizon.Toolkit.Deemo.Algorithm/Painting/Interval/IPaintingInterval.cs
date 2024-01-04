using SkiaSharp;

namespace Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
public interface IPaintingInterval
{
    IPaintingInterval Clone();
    IEnumerable<PaintingIntervalTag> GetTags(INotesPainter painter);
    float GetTextWidth(SKPaint paint);
}
