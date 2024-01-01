using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator;
internal sealed class Configuration
{
    private static readonly string ConfigFilePath = Path.Combine(Environment.CurrentDirectory, "Configurations", $"{nameof(InfoFileGenerator)}.default.json");

    private readonly ChartInfo _info;
    private readonly string? _exportPath;

    [JsonInclude]
    private DeemoPlayerSettingsView DemooPlayerSettings { get; }

    [JsonInclude]
    private string ExportPath => _exportPath ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    [JsonInclude]
    private string DefaultCharter { get => _info.Charter; set => _info.Charter = value; }

    private Configuration(ChartInfo info, string? exportPath = null)
    {
        _info = info;
        _exportPath = exportPath;
    }

    public static Configuration Load()
    {
        Configuration? result = null;
        if (File.Exists(ConfigFilePath))
            result = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(ConfigFilePath));

        return result ?? new(new());
    }

    public static void Save(ChartInfo info, string exportPath)
    {
        string json = JsonSerializer.Serialize(new Configuration(info, exportPath));
        File.WriteAllText(ConfigFilePath, json);
    }

    public void Deconstruct(out string exportPath, out ChartInfo chartInfo)
        => (exportPath, chartInfo) = (ExportPath, _info);

    private readonly struct DeemoPlayerSettingsView(ChartInfo info)
    {
        [JsonInclude] public bool UseMidi { get => info.UseMidi; set => info.UseMidi = value; }
        [JsonInclude] public string DefaultMidiFile { get => info.MidiFile; set => info.MidiFile = value; }
        [JsonInclude] public ChartDifficulty DefaultMidiDifficulty { get => info.MidiDifficulty; set => info.MidiDifficulty = value; }
        [JsonInclude] public string EasyJsonFile { get => info.JsonFiles[0]; set => info.JsonFiles[0] = value; }
        [JsonInclude] public string NormalJsonFile { get => info.JsonFiles[1]; set => info.JsonFiles[1] = value; }
        [JsonInclude] public string HardJsonFile { get => info.JsonFiles[2]; set => info.JsonFiles[2] = value; }
        [JsonInclude] public string ExtraJsonFile { get => info.JsonFiles[3]; set => info.JsonFiles[3] = value; }
        [JsonInclude] public string MusicFile { get => info.MusicFile; set => info.MusicFile = value; }
        [JsonInclude] public string PreviewFile { get => info.PreviewFile; set => info.PreviewFile = value; }
        [JsonInclude] public int Offset { get => info.Offset; set => info.Offset = value; }
        [JsonInclude] public string CoverFile { get => info.CoverFile; set => info.CoverFile = value; }
        [JsonInclude] public int AudioVolume { get => info.AudioVolume; set => info.AudioVolume = value; }
        [JsonInclude] public int PianoVolume { get => info.PianoVolume; set => info.PianoVolume = value; }
        [JsonInclude] public int Center { get => info.Center; set => info.Center = value; }
        [JsonInclude] public float Scale { get => info.Scale; set => info.Scale = value; }
    }
}
