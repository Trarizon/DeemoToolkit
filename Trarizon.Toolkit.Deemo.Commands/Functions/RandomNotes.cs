using CommandLine;
using Trarizon.Toolkit.Deemo.Algorithm;
using Trarizon.Toolkit.Deemo.ChartModels;
using Trarizon.Toolkit.Deemo.Commands.Core;

namespace Trarizon.Toolkit.Deemo.Commands.Functions;
[Verb(Verb)]
internal class RandomNotes : ChartConversionOptions
{
    public const string Verb = "rnd";
    protected override string Name => Verb;

    public override bool Async => false;

    [Option('s', "seed", Default = null)]
    public int? RandomSeed { get; set; }

    [Option('w', "random-size")]
    public bool RandomSize { get; set; }

    [Option('t', "time", Default = 60f / 240f / 6f, HelpText = "Treat notes as multi-tap, if the difference between their time is less than this number")]
    public float MultiTapTimeThreshold { get; set; }

    [Option('x', "pos", Default = 1f, HelpText = "The difference of multi-taps' pos will not less than this number")]
    public float MultiTapPositionThreshold { get; set; }

    protected override Result Convert(Chart chart)
    {
        chart.RandomNotes(MultiTapTimeThreshold, MultiTapPositionThreshold, RandomSize, RandomSeed);
        return chart;
    }
}
