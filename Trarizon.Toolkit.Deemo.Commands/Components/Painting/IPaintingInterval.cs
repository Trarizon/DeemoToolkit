using SkiaSharp;

namespace Trarizon.Toolkit.Deemo.Commands.Components.Painting;
public interface IPaintingInterval
{
    IPaintingInterval Clone();
    IEnumerable<PaintingIntervalTag> GetTags(ChartPainter painter);
    float GetTextWidth(SKPaint paint);
}
