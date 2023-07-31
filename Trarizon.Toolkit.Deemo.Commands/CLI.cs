using CommandLine;
using Trarizon.Library.Collections.CollectionQuery;
using Trarizon.Toolkit.Deemo.Commands.Core;

namespace Trarizon.Toolkit.Deemo.Commands;
internal static class CLI
{
    public static Parser Parser { get; } = new Parser();

    public static bool Parse<TOptions>(IEnumerable<string> args) where TOptions : IMultiInputOptions
    {
        if (!args.Any())
            return false;

        Parser.ParseArguments<TOptions>(args)
            .WithParsed(OnMultiInputOptionsParsed)
            .WithNotParsed(errs => Console.WriteLine($"Argument error: {string.Join(", ", errs.Select(err => err.Tag))}"));
        return true;
    }

    private static void OnMultiInputOptionsParsed<TOption>(TOption opt) where TOption : IMultiInputOptions
    {
        if (opt.Async && opt.Values.CountsMoreThan(1)) {
            List<Task> tasks = new();
            foreach (var value in opt.Values) {
                tasks.Add(Task.Run(() => Console.WriteLine(opt.Run(value))));
            }
            Task.WhenAll(tasks).RunSynchronously();
        }
        else {
            foreach (var value in opt.Values)
                Console.WriteLine(opt.Run(value));
        }
        Console.WriteLine("All inputs handled");
    }

    public static List<string> SplitAsArguments(this string input)
    {
        int start = 0;
        List<string> splits = new();

        while (start < input.Length) {
            if (char.IsWhiteSpace(input[start])) {
                start++;
                continue;
            }
            int end = start + 1;
            if (input[start] == '\"') {
                start++;
                while (end < input.Length) {
                    if (input[end] == '\"') {
                        if (end + 1 < input.Length && input[end + 1] == '\"') // escape
                            end++;
                        else // End
                            break;
                    }
                    end++;
                }

                splits.Add(Unescape(input.AsSpan(start, end - start)));
            }
            else {
                while (end < input.Length && !char.IsWhiteSpace(input[end]))
                    end++;

                splits.Add(input[start..end]);
            }
            start = end + 1;
        }
        return splits;

        static string Unescape(ReadOnlySpan<char> input)
        {
            Span<char> buffer = stackalloc char[input.Length];
            int count = 0;
            for (int i = 0; i < input.Length; i++) {
                if (input[i] == '"' && input[i + 1] == '"') {
                    count++;
                }
                buffer[count++] = input[i];
            }
            return new string(buffer.Slice(0, count));
        }
    }
}
