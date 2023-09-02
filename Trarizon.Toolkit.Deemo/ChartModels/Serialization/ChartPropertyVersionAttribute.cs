namespace Trarizon.Toolkit.Deemo.ChartModels.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
internal sealed class ChartPropertyVersionAttribute : Attribute
{
    public ChartPropertyVersions PropertyVersion { get; }

    public ChartPropertyVersionAttribute(ChartPropertyVersions propertyVersion)
    {
        PropertyVersion = propertyVersion;
    }
}
