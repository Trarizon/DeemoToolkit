namespace Trarizon.Library.CLParsing.Attributes;
public sealed class CLValueAttribute : CLParameterAttribute
{
    /// <summary>
    /// The order of input
    /// </summary>
    public int Order { get; }
    /// <summary>
    /// Max count of collection, if the property is not collection, 
    /// this property will be ignored and treat as 1
    /// </summary>
    public int MaxCount { get; }

    public CLValueAttribute(int order, int maxCount = int.MaxValue)
    {
        Order = order;
        MaxCount = maxCount;
    }
}
