using SkiaSharp;
using Trarizon.Toolkit.Deemo.Algorithm.Painting;
using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Algorithm;
public static class ChartPainter
{
	public static SKSurface Paint(IList<Note> notes, PaintingSettings? settings = null)
	{
		using var painter = new NotesPainter(notes, settings);
		return painter.Paint();
	}

	public static Task<SKSurface> PaintAsync(IList<Note> notes, PaintingSettings? settings = null)
	{
		using var painter = new NotesPainter(notes, settings);
		return painter.PaintAsync();
	}
}
