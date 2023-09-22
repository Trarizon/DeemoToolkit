using CommandLine;
using Trarizon.Toolkit.Deemo.Algorithm;
using Trarizon.Toolkit.Deemo.Algorithm.Painting;
using Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
using Trarizon.Toolkit.Deemo.ChartModels;
using Trarizon.Toolkit.Deemo.Commands.Core;

namespace Trarizon.Toolkit.Deemo.Commands.Functions;
[Verb(Verb)]
internal class ChartPainting : MultiChartInputOptions
{
    public const string Verb = "paint";
    protected override string Name => Verb;

    public override bool Async => true;

    [Option('l', "left")]
    public string? LeftInterval { get; set; }
    [Option('r', "right")]
    public string? RightInterval { get; set; }
    [Option('v', "version")]
    public int GameVersion { get; set; }
    [Option('m', "midi")]
    public bool ShowBackgroundMidi { get; set; }
    [Option("skip")]
    public bool SkipLeadingBlank { get; set; }

    [Option("lp")]
    public int LeftPadding { get; set; }
    [Option("tp")]
    public int TopPadding { get; set; }
    [Option("rp")]
    public int RightPadding { get; set; }
    [Option("bp")]
    public int BottomPadding { get; set; }

    [Option('g', "gap")]
    public int GapWidth { get; set; }

    [Option('s', "speed", Default = 1f)]
    public float Speed { get; set; }

    [Option('t', "time", Default = 20)]
    public int SegmentMainAreaTimeSeconds { get; set; }

    [Option('i', "enter")]
    public float SubAreaEnter { get; set; }
    [Option('o', "exit")]
    public float SubAreaExit { get; set; }

    protected override string Run(Chart chart, string sourcePath)
    {
        using var surface = ChartPainter.Paint(chart.Notes, new PaintingSettings {
            LeftInterval = ParseInterval(LeftInterval),
            RightInterval = ParseInterval(RightInterval),
            GameVersion = GetGameVersion(),
            ShowMidi = ShowBackgroundMidi,
            SkipLeadingBlank = SkipLeadingBlank,
            Padding = (LeftPadding, TopPadding, RightPadding, BottomPadding),
            SegmentsGapWidth = GapWidth,
            Speed = Speed,
            SegmentMainAreaTimeSeconds = SegmentMainAreaTimeSeconds,
            SubAreaTimes = (SubAreaEnter, SubAreaExit),
        });
        using var file = File.Create(sourcePath + ".png");
        surface.Snapshot().Encode().SaveTo(file);
        return $"{Path.GetFileName(sourcePath)} painted.";
    }

    static IPaintingInterval? ParseInterval(string? text)
    {
        if (text is null)
            return null;
        if (text.StartsWith("combo:") && int.TryParse(text.AsSpan(6), out var combo))
            return new ComboInterval(combo);
        if (text.StartsWith("time:") && float.TryParse(text.AsSpan(5), out var time))
            return new TimeInterval(time);
        return null;
    }

    GameVersion GetGameVersion() => GameVersion switch {
        1 => Deemo.GameVersion.DeemoReborn,
        2 => Deemo.GameVersion.DeemoII,
        _ => Deemo.GameVersion.Deemo,
    };
}
