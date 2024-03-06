using System.Diagnostics.CodeAnalysis;
using Trarizon.Library.Wrappers;
using Trarizon.TextCommand.Input;
using Trarizon.TextCommand.Input.Result;
using Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Commands.Functions;
internal static partial class ChartHandler
{
    private static void ErrorHandler(in ArgParsingErrors errors)
    {
        foreach (var err in errors) {
            switch (err.ErrorKind) {
                case ArgResultKind.ParameterNotSet:
                    Console.WriteLine($"Parameter {err.ParameterName} is required");
                    break;
                case ArgResultKind.ParsingFailed:
                    Console.WriteLine($"Cannot parse {err.RawInputSpan} into {err.ResultType}");
                    break;
            }
        }
    }

    private static bool TryParseInterval(InputArg input, [MaybeNullWhen(false)] out IPaintingInterval result)
    {
        var inputSpan = input.RawInputSpan;
        if (inputSpan.StartsWith("combo:") && int.TryParse(inputSpan[6..], out var combo)) {
            result = new ComboInterval(combo);
            return true;
        }
        if (inputSpan.StartsWith("time:") && int.TryParse(inputSpan[5..], out var time)) {
            result = new TimeInterval(time);
            return true;
        }
        result = default;
        return false;
    }

    private static bool TryParseGameVersion(InputArg input, out GameVersion gameVersion)
    {
        var inputSpan = input.RawInputSpan;
        if (int.TryParse(inputSpan, out var version)) {
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

    private static bool TryParseChart(InputArg input, out Result<(Chart, string), string> chartResult)
    {
        var inputFilePath = input.RawInput;
        if (!File.Exists(inputFilePath))
            chartResult = $"File not exist: {inputFilePath}";
        else if (!Chart.TryParseFromJson(File.ReadAllText(inputFilePath), out var chart))
            chartResult = $"Load chart failed: {inputFilePath}";
        else
            chartResult = (chart, inputFilePath);
        return true;
    }

    private static string HandleChart(Result<(Chart Chart, string InputPath), string> arg, Func<Chart, string, string> handler)
    {
        if (!arg.TryGetValue(out var chart, out var err)) {
            return err;
        }

        try {
            return handler(chart.Chart, chart.InputPath);
        } catch (Exception ex) {
            return $$"""
                Error {{{Path.GetFileName(chart.InputPath)}}}
                    {{ex.Message}}
                """;
        }
    }
}
