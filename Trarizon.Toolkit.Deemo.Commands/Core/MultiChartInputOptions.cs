using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Commands.Core;
internal abstract class MultiChartInputOptions : IMultiInputOptions
{
    public required IEnumerable<string> Values { get; set; }

    public abstract bool Async { get; }

    protected abstract string Name { get; }

    public string Run(string input)
    {
        if (!File.Exists(input))
            return $"File not exists: {input}.";
        else if (!Chart.TryParseFromJson(File.ReadAllText(input), out var chart))
            return $"Load chart failed: {input}";
        else {
            try {
                return Run(chart, input);
            } catch (Exception ex) {
                return $$"""
                    Error {{{Path.GetFileName(input)}}}:
                        {{ex.Message}}
                    """;
            }
        }
    }

    protected abstract string Run(Chart chart, string sourcePath);
}
