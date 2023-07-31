using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo;
public static class ChartExtensions
{
    public static bool IsLinkHead(this Note note) => note.IsSlide && note.PrevLink == null;

    public static bool IsVisible(this Note note) => note.Position is <= 2f and >= -2f;
}
