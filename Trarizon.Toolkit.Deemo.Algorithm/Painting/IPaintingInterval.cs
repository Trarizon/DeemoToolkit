using SkiaSharp;

namespace Trarizon.Toolkit.Deemo.Algorithm.Painting;
public interface IPaintingInterval
{
    IPaintingInterval Clone();
    IEnumerable<PaintingIntervalTag> GetTags(NotesPainter painter);
    float GetTextWidth(SKPaint paint);
}
