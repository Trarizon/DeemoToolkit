using System;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;
internal enum ChartDifficulty
{
    Easy,
    Normal,
    Hard,
    Extra,
    Expert = Extra,
}

internal static class ChartDifficultyExts
{
    public static string ToLowerCaseShortName(this ChartDifficulty difficulty)
        => difficulty switch {
            ChartDifficulty.Easy => "ez",
            ChartDifficulty.Normal => "nm",
            ChartDifficulty.Hard => "hd",
            ChartDifficulty.Extra => "ex",
            _ => throw new NotImplementedException(),
        };
}