namespace Trarizon.Library.CLParsing;
public sealed class CLSettings
{
    public static CLSettings Default { get; } = new();

    public string FullNamePrefix { get; set; } = "--";
    public string ShortNamePrefix { get; set; } = "-";
}
