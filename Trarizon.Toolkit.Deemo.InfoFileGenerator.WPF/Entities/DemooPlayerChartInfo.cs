using CommunityToolkit.Mvvm.ComponentModel;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;
public partial class DemooPlayerChartInfo : ObservableObject
{
    [NotifyPropertyChangedFor(nameof(UseJson))]
    [ObservableProperty] bool _useMidi = false;
    public bool UseJson => !UseMidi;

    [ObservableProperty] string _midiFileName = "Midi.mid";
    [ObservableProperty] ChartDifficulty _midiDifficulty = ChartDifficulty.Hard;

    [ObservableProperty] string _jsonEasyFileName = "Easy.json";
    [ObservableProperty] string _jsonNormalFileName = "Normal.json";
    [ObservableProperty] string _jsonHardFileName = "Hard.json";
    [ObservableProperty] string _jsonExtraFileName = "Extra.json";
    [ObservableProperty] string _musicFileName = "Music.mp3";
    [ObservableProperty] string _previewFileName = "Preview.mp3";
    [ObservableProperty] int _offset = 0;
    [ObservableProperty] string _coverFileName = "Cover.png";
    [ObservableProperty] int _audioVolume = 90;
    [ObservableProperty] int _pianoVolume = 90;
    [ObservableProperty] int _center = 60;
    [ObservableProperty] float _scale = 1.0f;

    public string GetJsonFileName(ChartDifficulty difficulty)
        => difficulty switch {
            ChartDifficulty.Easy => JsonEasyFileName,
            ChartDifficulty.Normal => JsonNormalFileName,
            ChartDifficulty.Hard => JsonHardFileName,
            ChartDifficulty.Extra => JsonExtraFileName,
            _ => ThrowHelper.ThrowInvalidOperationException<string>("Unknown difficulty"),
        };
}
