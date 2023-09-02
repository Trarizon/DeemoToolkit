﻿using System.Diagnostics;
using System.Diagnostics.Contracts;
using Trarizon.Toolkit.Deemo.ChartModels;
using Trarizon.Toolkit.Deemo.Commands;
using Trarizon.Toolkit.Deemo.Commands.Components.Painting;
using Trarizon.Toolkit.Deemo.Commands.Components.Painting.Interval;
using Trarizon.Toolkit.Deemo.Commands.Functions;

if (Chart.TryParseFromJson(File.ReadAllText(@"D:\Project Charts\Deenote\Music\&Deemo\5.Elysian Volitation.hard.txt"), out var chart)) {
    Console.WriteLine("Loaded");
     File.WriteAllText(@"D:\Project Charts\Deenote\Music\&Deemo\5.Elysian Volitation.hard.json", chart.ToJson(Trarizon.Toolkit.Deemo.ChartVersion.DeemoV2));
    //Console.WriteLine(Chart.TryParseFromJson(chart.ToJson(),out _));
}
else Console.WriteLine("failed");

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
