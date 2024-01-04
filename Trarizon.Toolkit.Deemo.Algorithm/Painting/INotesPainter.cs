using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Algorithm.Painting;
public interface INotesPainter
{
    IReadOnlyList<Note> Notes { get; }
    float MaxTime { get; }
}
