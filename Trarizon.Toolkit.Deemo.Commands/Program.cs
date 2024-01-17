using Trarizon.Toolkit.Deemo.Commands.Functions;

bool verbChangable = true;

if (args.Length > 0) {
    ChartHandler.PrevVerb = args[0];
    verbChangable = false;
}

string? defaultVerb = verbChangable ? null : args[0];
while (true) {
    Console.Write($"{defaultVerb}> ");
    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input)) {
        if(verbChangable) {
            defaultVerb = null;
        }
        continue;
    }

    foreach (var res in ChartHandler.Run($"{defaultVerb} {input}") ?? []) {
        Console.WriteLine(res);
    }

    if (verbChangable) {
        defaultVerb = ChartHandler.PrevVerb;
    }
}

