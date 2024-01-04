using SkiaSharp;
using Trarizon.Toolkit.Deemo.Algorithm.Painting;
using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Algorithm;
public static class ChartPainter
{
    public static SKSurface Paint(IReadOnlyList<Note> notes, PaintingSettings? settings = null)
    {
        using var painter = new NotesPainter(notes, settings ?? PaintingSettings.Default);
        return painter.Paint();
    }

    public static Task<SKSurface> PaintAsync(IReadOnlyList<Note> notes, PaintingSettings? settings = null)
    {
        using var painter = new NotesPainter(notes, settings ?? PaintingSettings.Default);
        return painter.PaintAsync();
    }

    public static void PaintTo(IReadOnlyList<Note> notes, Stream output, PaintingSettings? settings = null)
    {
        using var surface = Paint(notes, settings);
        using var stream = surface.Snapshot().Encode().AsStream();
        stream.CopyTo(output);
    }
}
