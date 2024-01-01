using System;
using System.Text;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;
partial class ChartInfo
{
    public string GetIni()
    {
        var sb = new StringBuilder($"""
            [Song]
            Name={MusicName}

            """);
        AddProperty("Artist", Composer);
        AddProperty("Noter", Charter);
        AddProperty("Easy", Levels[0]);
        AddProperty("Normal", Levels[1]);
        AddProperty("Hard", Levels[2]);
        AddProperty("Extra", Levels[3]);

        sb.Length--;
        return sb.ToString();

        void AddProperty(string name, string? value)
        {
            if (!string.IsNullOrEmpty(value))
                sb.AppendLine($"{name}={value}");
        }
    }

    public ChartAdaptedArray<string?> GetTxts()
    {
        ChartAdaptedArray<string?> result = default;

        if (UseMidi) {
            if (MidiDifficulty is < ChartDifficulty.Easy or > ChartDifficulty.Extra)
                throw new InvalidOperationException("Unknown difficulty");
            result[MidiDifficulty] = GetText(MidiDifficulty);
        }
        else {
            for (int i = 0; i < 4; i++) {
                if (!string.IsNullOrEmpty(Levels[i]))
                    result[i] = GetText((ChartDifficulty)i);
            }
        }

        return result;

        string GetText(ChartDifficulty difficulty)
            => $"""
            {(UseMidi
                ? $"MIDI {MidiFile}"
                : $"JSON {JsonFiles[difficulty]}")}
            AUDIO {MusicFile}
            PREVIEW {PreviewFile}
            OFFSET {Offset}
            COVER {CoverFile}
            TITLE {MusicName}
            DIFFICULTY {(int)difficulty}
            LEVEL {Levels[difficulty]}
            AUDIOVOLUME {AudioVolume}
            PIANOVOLUME {PianoVolume}
            COMPOSER {Composer}
            CENTER {Center}
            SCALE {Scale}
            """;
    }
}
