namespace Trarizon.Toolkit.Deemo.Algorithm.Painting;
public sealed class PaintingSettings
{
    public static PaintingSettings Default { get; } = new PaintingSettings();

    public IPaintingInterval? LeftInterval { get; set; }
    public IPaintingInterval? RightInterval { get; set; }

    public GameVersion GameVersion { get; set; }
    public bool ShowMidi { get; set; }
    public bool SkipLeadingBlank { get; set; }

    /// <summary>
    /// Preserved empty space
    /// </summary>
    public (int Left, int Top, int Right, int Bottom) Padding { get; set; }

    /// <summary>
    /// Distance between 2 segments
    /// </summary>
    public int SegmentsGapWidth { get; set; }

    /// <summary>
    /// Speed, default <see langword="1f"/>
    /// </summary>
    public float Speed { get; set; } = 1f;

    /// <summary>
    /// Time of a segment, default <see langword="20"/>
    /// </summary>
    public int SegmentMainAreaTimeSeconds { get; set; } = 20;
    /// <summary>
    /// Transition before and after main segment
    /// </summary>
    public (float Enter,float Exit) SubAreaTimes { get; set; }
}

