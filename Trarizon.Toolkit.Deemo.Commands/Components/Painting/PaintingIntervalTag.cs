namespace Trarizon.Toolkit.Deemo.Commands.Components.Painting;
public readonly struct PaintingIntervalTag
{
    public readonly float Time;
    public readonly string Text;

    public PaintingIntervalTag(float time, string text)
    {
        Time = time;
        Text = text;
    }
}
