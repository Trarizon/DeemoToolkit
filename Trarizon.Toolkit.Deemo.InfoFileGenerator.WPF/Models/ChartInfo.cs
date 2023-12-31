using System.Text;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Models;
public sealed class ChartInfo
{
    public BasicChartInfo Basic { get; }
    public DemooPlayerChartInfo DemooPlayer { get; }

    public ChartInfo(BasicChartInfo basicChartInfo, DemooPlayerChartInfo demooPlayerChartInfo)
    {
        Basic = basicChartInfo;
        DemooPlayer = demooPlayerChartInfo;
    }

    public string GetIni()
    {
        var sb = new StringBuilder($"""
            [Song]
            Name={Basic.MusicName}

            """);
        AddProperty(sb, "Artist", Basic.Composer);
        AddProperty(sb, "Noter", Basic.Charter);
        AddProperty(sb, "Easy", Basic.LevelEasy);
        AddProperty(sb, "Normal", Basic.LevelNormal);
        AddProperty(sb, "Hard", Basic.LevelHard);
        AddProperty(sb, "Extra", Basic.LevelExtra);
        AddProperty(sb, "Ultra", Basic.LevelExtra);

        sb.Length--;
        return sb.ToString();

        static void AddProperty(StringBuilder sb, string levelName, string levelValue)
        {
            if (!string.IsNullOrEmpty(levelValue))
                sb.AppendLine($"{levelName}={levelValue}");
        }
    }

    public string?[] GetTxts()
    {
        string?[] res = new string?[4];
        if (DemooPlayer.UseMidi) {
            var diff = (int)DemooPlayer.MidiDifficulty;
            if (diff is < (int)ChartDifficulty.Easy or > (int)ChartDifficulty.Extra)
                ThrowHelper.ThrowInvalidOperationException("Unknown difficulty");
            res[diff] = GetTxtText(DemooPlayer.MidiDifficulty);
        }
        else {
            if (!string.IsNullOrEmpty(Basic.LevelEasy)) res[0] = GetTxtText(ChartDifficulty.Easy);
            if (!string.IsNullOrEmpty(Basic.LevelNormal)) res[1] = GetTxtText(ChartDifficulty.Normal);
            if (!string.IsNullOrEmpty(Basic.LevelHard)) res[2] = GetTxtText(ChartDifficulty.Hard);
            if (!string.IsNullOrEmpty(Basic.LevelExtra)) res[3] = GetTxtText(ChartDifficulty.Extra);
        }
        return res;
    }

    private string GetTxtText(ChartDifficulty difficulty)
        => $"""
        {(DemooPlayer.UseMidi
            ? $"MIDI {DemooPlayer.MidiFileName}"
            : $"JSON {DemooPlayer.GetJsonFileName(difficulty)}")}
        AUDIO {DemooPlayer.MusicFileName}
        PREVIEW {DemooPlayer.PreviewFileName}
        OFFSET {DemooPlayer.Offset}
        COVER {DemooPlayer.CoverFileName}
        TITLE {Basic.MusicName}
        DIFFICULTY {(int)difficulty}
        LEVEL {Basic.GetLevel(difficulty)}
        AUDIOVOLUME {DemooPlayer.AudioVolume}
        PIANOVOLUME {DemooPlayer.PianoVolume}
        COMPOSER {Basic.Composer}
        CENTER {DemooPlayer.Center}
        SCALE {DemooPlayer.Scale}
        """;


}
