using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Commands.Core;
internal abstract class ChartConversionOptions : MultiChartInputOptions
{
    protected override string Run(Chart chart, string sourcePath)
    {
        var res = Convert(chart);
        File.WriteAllText(sourcePath.Insert(sourcePath.LastIndexOf('.'), $".{Name}"), res.Chart.ToJson());
        if (res.ExtraInfos.Length == 0)
            return $"Converted {{{Path.GetFileName(sourcePath)}}}.";
        else
            return $$"""
                <{{Name}}> Converted {{{Path.GetFileName(sourcePath)}}}:
                    {{{string.Join(", ", res.ExtraInfos.Select(kv => $"{kv.Key}: {kv.Value}"))}}}
                """;
    }

    protected abstract Result Convert(Chart chart);
}
