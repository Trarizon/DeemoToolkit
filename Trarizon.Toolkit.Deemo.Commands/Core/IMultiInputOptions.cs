using CommandLine;

namespace Trarizon.Toolkit.Deemo.Commands.Core;
internal interface IMultiInputOptions
{
    [Value(0)]
    IEnumerable<string> Values { get; set; }
    
    bool Async { get; }

    string Run(string input);
}
