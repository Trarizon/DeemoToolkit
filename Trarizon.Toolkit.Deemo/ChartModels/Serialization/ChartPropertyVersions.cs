namespace Trarizon.Toolkit.Deemo.ChartModels.Serialization;
internal enum ChartPropertyVersions
{
    DeemoV1 = 1,
    DeemoV2 = 2,
    DeemoV3 = 4,
    Deemo = DeemoV1 | DeemoV2 | DeemoV3,

    DeemoReborn = 8,

    DeemoIIV1 = 16,
    DeemoIIV2 = 32,
    DeemoII = DeemoIIV1 | DeemoIIV2,

    All = Deemo | DeemoReborn | DeemoII,
}
