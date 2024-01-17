using Trarizon.TextCommand.Attributes;
using Trarizon.TextCommand.Attributes.Parameters;
using Trarizon.Toolkit.Deemo.Algorithm;
using Trarizon.Toolkit.Deemo.Algorithm.Painting;
using Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;

namespace Trarizon.Toolkit.Deemo.Commands.Functions;
partial class ChartHandler
{
    public static string? PrevVerb;

    [Execution]
    public static partial IEnumerable<string>? Run(string input);

    [Executor("paint")]
    private static IEnumerable<string> Paint(
        [MultiValue] string[] inputPaths,
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

        foreach (var inputPath in inputPaths) {
            yield return HandleChart(inputPath, chart =>
            {
                using var output = File.Create($"{inputPath}.png");
                ChartPainter.PaintTo(chart.Notes, output, settings);
                return $"{Path.GetFileName(inputPath)} painted.";
            });
        }
    }

    [Executor("rnd")]
    private static IEnumerable<string> RandomNotes(
        [MultiValue] string[] inputPaths,
        [Option("s", Name = "seed")] int? seed,
        [Flag("w", Name = "random-size")] bool randomSize,
        [Option("t", Name = "time")] float? multiTapTimeThreshold,
        [Option("x", Name = "pos")] float? multiTapPositionThreshold)
    {
        PrevVerb = "rnd";
        foreach (var inputPath in inputPaths) {
            yield return HandleChart(inputPath, chart =>
            {
                chart.RandomNotes(
                    multiTapTimeThreshold ?? 60f / 240f / 6f,
                    multiTapPositionThreshold ?? 1,
                    randomSize,
                    seed);
                File.WriteAllText(inputPath.Insert(inputPath.LastIndexOf('.'), ".rnd"), chart.ToJson());
                return $"Random converted: {Path.GetFileName(inputPath)}";
            });
        }
    }

    [Executor("piano")]
    private static IEnumerable<string> RearrangeByPitch(
        [MultiValue] string[] inputPaths,
        [Flag("f", Name = "fix")] bool fixRange)
    {
        PrevVerb = "piano";
        foreach (var inputPath in inputPaths) {
            yield return HandleChart(inputPath, chart =>
            {
                var newChart = chart.RearrangeByPitch(fixRange);
                File.WriteAllText(inputPath.Insert(inputPath.LastIndexOf('.'), ".rnd"), newChart.ToJson());
                return $"Rearranged by pitch: {Path.GetFileName(inputPath)}";
            });
        }
    }

    [Executor("black")]
    private static IEnumerable<string> ToAllClick([MultiValue] string[] inputPaths)
    {
        PrevVerb = "black";
        foreach (var inputPath in inputPaths) {
            yield return HandleChart(inputPath, chart =>
            {
                chart.ToAllClick();
                File.WriteAllText(inputPath.Insert(inputPath.LastIndexOf('.'), ".rnd"), chart.ToJson());
                return $"To all click: {Path.GetFileName(inputPath)}";
            });
        }
    }
}
