namespace Trarizon.Library.CLParsing.Attributes;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
public abstract class CLParameterAttribute : Attribute
{
    /// <summary>
    /// Default value when no argument set
    /// </summary>
    public object? Default { get; set; }

    protected CLParameterAttribute() { }
}
