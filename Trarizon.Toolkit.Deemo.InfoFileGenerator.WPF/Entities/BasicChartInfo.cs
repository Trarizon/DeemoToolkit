using CommunityToolkit.Mvvm.ComponentModel;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;
public partial class BasicChartInfo : ObservableObject
{
    [ObservableProperty] string _musicName = "";
    [ObservableProperty] string _composer = "";
    [ObservableProperty] string _charter = "";
    [ObservableProperty] string _levelEasy = "";
    [ObservableProperty] string _levelNormal = "";
    [ObservableProperty] string _levelHard = "";
    [ObservableProperty] string _levelExtra = "";

    public string GetLevel(ChartDifficulty difficulty)
        => difficulty switch {
            ChartDifficulty.Easy => LevelEasy,
            ChartDifficulty.Normal => LevelNormal,
            ChartDifficulty.Hard => LevelHard,
            ChartDifficulty.Extra => LevelExtra,
            _ => ThrowHelper.ThrowInvalidOperationException<string>("Unknown difficulty"),
        };
}
