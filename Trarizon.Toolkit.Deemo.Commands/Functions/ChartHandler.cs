using System.Diagnostics.CodeAnalysis;
using Trarizon.Library.Wrappers;
using Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Commands.Functions;
internal static partial class ChartHandler
{
    private static bool TryParseInterval(ReadOnlySpan<char> input, [MaybeNullWhen(false)] out IPaintingInterval result)
    {
        if (input.StartsWith("combo:") && int.TryParse(input[6..], out var combo)) {
            result = new ComboInterval(combo);
            return true;
        }
        if (input.StartsWith("time:") && int.TryParse(input[5..], out var time)) {
            result = new TimeInterval(time);
            return true;
        }
        result = default;
        return false;
    }

    private static bool TryParseGameVersion(ReadOnlySpan<char> input, out GameVersion gameVersion)
    {
        if (int.TryParse(input, out var version)) {
            gameVersion = version switch {
                1 => Deemo.GameVersion.DeemoReborn,
                2 => Deemo.GameVersion.DeemoII,
                _ => Deemo.GameVersion.Deemo,
            };
            return true;
        }
        gameVersion = default;
        return false;
    }

    private static Result<Chart, string> LoadChart(string inputFilePath)
    {
        if (!File.Exists(inputFilePath))
            return $"File not exist: {inputFilePath}";
        else if (!Chart.TryParseFromJson(File.ReadAllText(inputFilePath), out var chart))
            return $"Load chart failed: {inputFilePath}";
        else
            return chart;
    }

    private static string HandleChart(string inputPath, Func<Chart, string> handler)
    {
        if (!LoadChart(inputPath).TryGetValue(out var chart, out var err)) {
            return err;
        }
        else {
            try {
                return handler(chart);
            } catch (Exception ex) {
                return $$"""
                        Error {{{Path.GetFileName(inputPath)}}}
                            {{ex.Message}}
                        """;
            }
        }
    }
}
