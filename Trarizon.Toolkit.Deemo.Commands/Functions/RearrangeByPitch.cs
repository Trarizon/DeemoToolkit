using CommandLine;
using Trarizon.Toolkit.Deemo.ChartModels;
using Trarizon.Toolkit.Deemo.Commands.Components;
using Trarizon.Toolkit.Deemo.Commands.Core;

namespace Trarizon.Toolkit.Deemo.Commands.Functions;
[Verb(Verb)]
internal class RearrangeByPitch : ChartConversionOptions
{
    public const string Verb = "piano";
    protected override string Name => Verb;

    public override bool Async => false;

    [Option('f', "fix")]
    public bool FixRange { get; set; }

    protected override Result Convert(Chart chart)
    {
        return chart.RearrangeByPitch(FixRange);
    }
}
