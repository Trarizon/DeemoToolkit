using CommandLine;
using Trarizon.Toolkit.Deemo.Algorithm;
using Trarizon.Toolkit.Deemo.ChartModels;
using Trarizon.Toolkit.Deemo.Commands.Core;

namespace Trarizon.Toolkit.Deemo.Commands.Functions;
[Verb(Verb)]
internal class ToAllClickChart : ChartConversionOptions
{
    public const string Verb = "black";

    public override bool Async => false;

    protected override string Name => Verb;

    protected override Result Convert(Chart chart)
    {
        chart.ToAllClick();
        return chart;
    }
}
