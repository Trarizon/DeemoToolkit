namespace Trarizon.Library.CLParsing.Attributes;
public sealed class CLOptionAttribute : CLParameterAttribute
{
    /// <summary>
    /// Full name of this option
    /// </summary>
    public string FullName { get; }
    /// <summary>
    /// Short name of this option
    /// </summary>
    public string? ShortName { get; }

    public CLOptionAttribute(string fullName, string? shortName = null)
    {
        FullName = fullName;
        ShortName = shortName;
    }
}
