using Trarizon.Library.Wrappers;
using Trarizon.TextCommand.Attributes;
using Trarizon.TextCommand.Attributes.Parameters;
using Trarizon.Toolkit.Deemo.Algorithm;
using Trarizon.Toolkit.Deemo.Algorithm.Painting;
using Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Commands.Functions;
partial class ChartHandler
{
    public static string? PrevVerb;

    [Execution(ErrorHandler = nameof(ErrorHandler))]
    public static partial IEnumerable<string>? Run(string input);

    [Executor("img")]
    [Executor("paint")]
    private static IEnumerable<string> Paint(
        [MultiValue(Parser = nameof(TryParseChart))] Result<(Chart Chart, string InputPath), string>[] charts,
        [Option("l", Name = "left", Parser = nameof(TryParseInterval))] IPaintingInterval? leftInterval,
        [Option("r", Name = "right", Parser = nameof(TryParseInterval))] IPaintingInterval? rightInterval,
        [Option("v", Name = "version", Parser = nameof(TryParseGameVersion))] GameVersion gameVersion,
        [Flag("m", Name = "midi")] bool showBackingMidi,
        [Flag("skip")] bool skipLeadingBlank,
        [Option("lp")] int leftPadding,
        [Option("tp")] int topPadding,
        [Option("rp")] int rightPadding,
        [Option("bp")] int bottomPadding,
        [Option("g", Name = "gap")] int gapWidth,
        [Option("s", Name = "speed")] float speed,
        [Option("t", Name = "time")] int segmentMainAreaTime_s,
        [Option("i", Name = "in")] float subAreaIn,
        [Option("o", Name = "out")] float subAreaOut)
    {
        PrevVerb = "paint";
        var settings = new PaintingSettings {
            LeftInterval = leftInterval,
            RightInterval = rightInterval,
            GameVersion = gameVersion,
            ShowMidi = showBackingMidi,
            SkipLeadingBlank = skipLeadingBlank,
            Padding = (leftPadding, topPadding, rightPadding, bottomPadding),
            SegmentsGapWidth = gapWidth,
            Speed = speed == 0f ? 1f : speed,
            SegmentMainAreaTimeSeconds = segmentMainAreaTime_s == 0 ? 20 : segmentMainAreaTime_s,
            SubAreaTimes = (subAreaIn, subAreaOut),
        };

        foreach (var chart in charts) {
            yield return HandleChart(chart, (chart, path) =>
            {
                using var output = File.Create($"{path}.png");
                ChartPainter.PaintTo(chart.Notes, output, settings);
                return $"{Path.GetFileName(path)} painted.";
            });
        }
    }

    [Executor("rnd")]
    private static IEnumerable<string> RandomNotes(
        [MultiValue(Parser = nameof(TryParseChart))] Result<(Chart Chart, string InputPath), string>[] charts,
        [Option("s", Name = "seed")] int? seed,
        [Flag("w", Name = "random-size")] bool randomSize,
        [Option("t", Name = "time")] float? multiTapTimeThreshold,
        [Option("x", Name = "pos")] float? multiTapPositionThreshold)
    {
        PrevVerb = "rnd";
        foreach (var chart in charts) {
            yield return HandleChart(chart, (chart, path) =>
            {
                chart.RandomNotes(
                    multiTapTimeThreshold ?? 60f / 240f / 6f,
                    multiTapPositionThreshold ?? 1,
                    randomSize,
                    seed);
                File.WriteAllText(path.Insert(path.LastIndexOf('.'), ".rnd"), chart.ToJson());
                return $"Random converted: {Path.GetFileName(path)}";
            });
        }
    }

    [Executor("piano")]
    private static IEnumerable<string> RearrangeByPitch(
        [MultiValue(Parser = nameof(TryParseChart))] Result<(Chart Chart, string InputPath), string>[] charts,
        [Flag("f", Name = "fix")] bool fixRange)
    {
        PrevVerb = "piano";
        foreach (var chart in charts) {
            yield return HandleChart(chart, (chart, path) =>
            {
                var newChart = chart.RearrangeByPitch(fixRange);
                File.WriteAllText(path.Insert(path.LastIndexOf('.'), ".piano"), newChart.ToJson());
                return $"Rearranged by pitch: {Path.GetFileName(path)}";
            });
        }
    }

    [Executor("black")]
    private static IEnumerable<string> ToAllClick(
        [MultiValue(Parser = nameof(TryParseChart))] Result<(Chart Chart, string InputPath), string>[] charts)
    {
        PrevVerb = "black";
        foreach (var chart in charts) {
            yield return HandleChart(chart, (chart, path) =>
            {
                chart.ToAllClick();
                File.WriteAllText(path.Insert(path.LastIndexOf('.'), ".black"), chart.ToJson());
                return $"To all click: {Path.GetFileName(path)}";
            });
        }
    }
}
