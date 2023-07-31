using System.Diagnostics;
using Trarizon.Toolkit.Deemo.ChartModels;
using Trarizon.Toolkit.Deemo.Commands;
using Trarizon.Toolkit.Deemo.Commands.Components.Painting;
using Trarizon.Toolkit.Deemo.Commands.Components.Painting.Interval;
using Trarizon.Toolkit.Deemo.Commands.Functions;

//string inputstr = @"D:\Project Charts\Collection\Forest of Clock\10.json";
//if (Chart.TryParseFromJson(File.ReadAllText(inputstr), out var chart)) {
//    var painter = new ChartPainter(chart.Notes, new PaintingSettings {
//        Padding = (50, 50, 50, 50),
//        SegmentMainAreaTimeSeconds = 15,
//        SubAreaTimes = (1f, 1f),
//        LeftInterval = new TimeInterval(5f),
//        RightInterval = new ComboInterval(20),
//        SegmentsGapWidth = 50,
//        GameVersion = Trarizon.Toolkit.Deemo.GameVersion.Deemo,
//    });
//    Console.WriteLine("Drawing");
//    var surface = await painter.PaintAsync();
//    Console.WriteLine("Drown.");
//    using var file = File.Create(inputstr + ".png");
//    surface.Snapshot().Encode().SaveTo(file);
//}
//return;


bool verbChangable = true;

if (args.Length > 0) {
    if (Run(args[0], args.Skip(1)))
        return;
    else
        verbChangable = false;
}

string? defaultVerb = verbChangable ? null : args[0];
while (true) {
    Console.Write($"{defaultVerb}> ");
    string input = Console.ReadLine() ?? string.Empty;
    List<string> inputArgs = input.SplitAsArguments();

    if (defaultVerb is null) {
        Debug.Assert(verbChangable);

        if (inputArgs.Count == 0)
            continue;

        string verb = inputArgs[0];
        if (!Run(verb, inputArgs.Skip(1))) {
            // Enter verb to set default verb
            defaultVerb = verb;
        }
    }
    else {
        if (!Run(defaultVerb, inputArgs)) {
            // Enter empty to reset default verb
            defaultVerb = null;
        }
    }
}


static bool Run(string verb, IEnumerable<string> args)
    => verb switch {
        RearrangeByPitch.Verb => CLI.Parse<RearrangeByPitch>(args),
        RandomNotes.Verb => CLI.Parse<RandomNotes>(args),
        ChartPainting.Verb => CLI.Parse<ChartPainting>(args),
        ToAllClickChart.Verb => CLI.Parse<ToAllClickChart>(args),
        _ => OnUnknownVerb(verb),
    };

static bool OnUnknownVerb(string verb)
{
    if (!string.IsNullOrEmpty(verb))
        Console.WriteLine($"Unknown command {verb}");
    return true;
}
