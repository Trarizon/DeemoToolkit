using CommunityToolkit.Mvvm.ComponentModel;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.ViewModels;
internal sealed class TxtInfoViewModel(ChartInfo info) : ObservableObject
{
    public bool UseMidi
    {
        get => info.UseMidi;
        set {
            if (SetProperty(ref info.UseMidi, value))
                OnPropertyChanged(nameof(UseJson));
        }
    }

    public bool UseJson => !UseMidi;

    public string MidiFile
    {
        get => info.MidiFile;
        set => SetProperty(ref info.MidiFile, value);
    }

    public ChartDifficulty MidiDifficulty
    {
        get => info.MidiDifficulty;
        set => SetProperty(ref info.MidiDifficulty, value);
    }

    public string EasyJsonFile
    {
        get => info.JsonFiles[0];
        set => SetProperty(ref info.JsonFiles[0], value);
    }

    public string NormalJsonFile
    {
        get => info.JsonFiles[1];
        set => SetProperty(ref info.JsonFiles[1], value);
    }

    public string HardJsonFile
    {
        get => info.JsonFiles[2];
        set => SetProperty(ref info.JsonFiles[2], value);
    }

    public string ExtraJsonFile
    {
        get => info.JsonFiles[3];
        set => SetProperty(ref info.JsonFiles[3], value);
    }

    public string MusicFile
    {
        get => info.MusicFile;
        set => SetProperty(ref info.MusicFile, value);
    }

    public string PreviewFile
    {
        get => info.PreviewFile;
        set => SetProperty(ref info.PreviewFile, value);
    }

    public string CoverFile
    {
        get => info.CoverFile;
        set => SetProperty(ref info.CoverFile, value);
    }

    public int Offset
    {
        get => info.Offset;
        set => SetProperty(ref info.Offset, value);
    }

    public int AudioVolume
    {
        get => info.AudioVolume;
        set => SetProperty(ref info.AudioVolume, value);
    }

    public int PianoVolume
    {
        get => info.PianoVolume;
        set => SetProperty(ref info.PianoVolume, value);
    }

    public int Center
    {
        get => info.Center;
        set => SetProperty(ref info.Center, value);
    }

    public float Scale
    {
        get => info.Scale;
        set => SetProperty(ref info.Scale, value);
    }
}
