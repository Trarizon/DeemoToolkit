namespace Trarizon.Toolkit.Deemo.ChartModels.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
internal sealed class ChartPropertyVersionAttribute : Attribute
{
    public ChartPropertyVersion PropertyVersion { get; }

    public ChartPropertyVersionAttribute(ChartPropertyVersion propertyVersion)
    {
        PropertyVersion = propertyVersion;
    }
}
