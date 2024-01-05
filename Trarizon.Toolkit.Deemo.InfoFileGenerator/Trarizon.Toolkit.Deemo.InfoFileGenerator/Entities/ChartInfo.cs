namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;
internal sealed partial class ChartInfo
{
    #region Basic info

    public string MusicName = "";
    public string Composer = "";
    public string Charter = "";
    public ChartAdaptedArray<string> Levels = ["", "", "", ""];

    #endregion

    #region DemooPlayer info

    public bool UseMidi = false;

    public string MidiFile = "Midi.mid";
    public ChartDifficulty MidiDifficulty = ChartDifficulty.Hard;

    public ChartAdaptedArray<string> JsonFiles = ["Easy.json", "Normal.json", "Hard.json", "Extra.json"];
    public string MusicFile = "Music.mp3";
    public string PreviewFile = "Preview.mp3";
    public string CoverFile = "Cover.png";

    public int Offset = 0;
    public int AudioVolume = 90;
    public int PianoVolume = 90;
    public int Center = 60;
    public float Scale = 1f;

    #endregion
}
