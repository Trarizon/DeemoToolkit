using Newtonsoft.Json;
using System;
using System.IO;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Models;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator;
[JsonObject(MemberSerialization.OptIn)]
internal sealed class Configuration
{
    private static string ConfigDir => Path.Combine(Environment.CurrentDirectory, "Configurations");
    private static string ConfigFileName => Path.Combine(ConfigDir, $"{nameof(InfoFileGenerator)}.json");

    public static string DefaultExportFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    public static Configuration Instance { get; private set; }

    public DemooPlayerChartInfo DemooPlayerChartInfo => DemooplayerSettings.ChartInfo;

    [JsonProperty]
    public string ExportPath { get; private set; }

    [JsonProperty]
    public string DefaultCharter { get; set; }

    [JsonProperty]
    private DemooplayerSettingsClass DemooplayerSettings { get; }

    static Configuration()
    {
        var file = ConfigFileName;
        string json = File.Exists(file) ? File.ReadAllText(file) : string.Empty;
        Instance = JsonConvert.DeserializeObject<Configuration>(json) ?? new(new(), DefaultExportFolderPath,"");
    }

    public Configuration(DemooPlayerChartInfo chartInfo, string exportPath, string defaultCharter)
    {
        DefaultCharter = defaultCharter;
        ExportPath = exportPath;
        DemooplayerSettings = new(chartInfo);
    }

    [JsonConstructor]
    private Configuration() : this(new(), "", "") { }

    public static void Save(Configuration configuration)
    {
        if (!Directory.Exists(ConfigDir))
            Directory.CreateDirectory(ConfigDir);

        File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(configuration));
        Instance = configuration;
    }

    [JsonObject]
    private class DemooplayerSettingsClass
    {
        [JsonIgnore]
        public DemooPlayerChartInfo ChartInfo { get; }

        public bool UseMidi { get => ChartInfo.UseMidi; set => ChartInfo.UseMidi = value; }
        public string DefaultMidiFile { get => ChartInfo.MidiFileName; set => ChartInfo.MidiFileName = value; }
        public ChartDifficulty DefaultMidiDifficulty { get => ChartInfo.MidiDifficulty; set => ChartInfo.MidiDifficulty = value; }
        public string EasyJsonFile { get => ChartInfo.JsonEasyFileName; set => ChartInfo.JsonEasyFileName = value; }
        public string NormalJsonFile { get => ChartInfo.JsonNormalFileName; set => ChartInfo.JsonNormalFileName = value; }
        public string HardJsonFile { get => ChartInfo.JsonHardFileName; set => ChartInfo.JsonHardFileName = value; }
        public string ExtraJsonFile { get => ChartInfo.JsonExtraFileName; set => ChartInfo.JsonExtraFileName = value; }
        public string MusicFile { get => ChartInfo.MusicFileName; set => ChartInfo.MusicFileName = value; }
        public string PreviewFile { get => ChartInfo.PreviewFileName; set => ChartInfo.PreviewFileName = value; }
        public int Offset { get => ChartInfo.Offset; set => ChartInfo.Offset = value; }
        public string CoverFile { get => ChartInfo.CoverFileName; set => ChartInfo.CoverFileName = value; }
        public int AudioVolume { get => ChartInfo.AudioVolume; set => ChartInfo.AudioVolume = value; }
        public int PianoVolume { get => ChartInfo.PianoVolume; set => ChartInfo.PianoVolume = value; }
        public int Center { get => ChartInfo.Center; set => ChartInfo.Center = value; }
        public float Scale { get => ChartInfo.Scale; set => ChartInfo.Scale = value; }

        public DemooplayerSettingsClass(DemooPlayerChartInfo chartInfo) => ChartInfo = chartInfo;
    }
}
