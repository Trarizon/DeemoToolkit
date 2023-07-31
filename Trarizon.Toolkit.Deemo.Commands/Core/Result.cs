using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Commands.Core;
internal record class Result(
    Chart Chart,
    (string Key, string Value)[] ExtraInfos)
{
    public Result(Chart chart) : this(chart, Array.Empty<(string, string)>()) { }

    public static implicit operator Result(Chart chart) => new(chart);
}
