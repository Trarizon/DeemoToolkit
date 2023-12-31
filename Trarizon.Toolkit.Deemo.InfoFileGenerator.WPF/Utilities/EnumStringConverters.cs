using System;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;
internal static class EnumStringConverters
{
    public static string ShortNameLowerCase(this ChartDifficulty difficulty)
        => difficulty switch {
            ChartDifficulty.Easy => "ez",
            ChartDifficulty.Normal => "nm",
            ChartDifficulty.Hard => "hd",
            ChartDifficulty.Extra => "ex",
            _ => throw new NotImplementedException(),
        };
}
