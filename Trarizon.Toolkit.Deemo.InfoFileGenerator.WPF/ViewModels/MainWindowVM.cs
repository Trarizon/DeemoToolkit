using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.ViewModels;
internal partial class MainWindowVM : ObservableObject
{
    [ObservableProperty] ChartInfo _chartInfo;
    [ObservableProperty] string _exportPath;

    private FolderFileSet? _folderFiles;

    public IEnumerable<string> FolderFiles => _folderFiles ?? Enumerable.Empty<string>();

    public MainWindowVM()
    {
        _chartInfo = new(
            new BasicChartInfo { Charter = Configuration.Instance.DefaultCharter },
            Configuration.Instance.DemooPlayerChartInfo);
        _exportPath = Configuration.Instance.ExportPath;
    }

    [RelayCommand]
    void ExportIni()
    {
        File.WriteAllText(
            Path.Combine(ExportPath, $"{ChartInfo.Basic.MusicName}.ini"),
            ChartInfo.GetIni());
    }

    [RelayCommand]
    void ExportTxt()
    {
        var texts = ChartInfo.GetTxts();
        for(int i = 0; i < texts.Length; i++) {
            if (texts[i] is null)
                continue;

            File.WriteAllText(
                Path.Combine(ExportPath, $"{ChartInfo.Basic.MusicName}.{((ChartDifficulty)i).ShortNameLowerCase()}.txt"),
                texts[i]);
        }
    }

    [RelayCommand]
    void ExportAsConfiguration()
    {
        Configuration.Save(new(ChartInfo.DemooPlayer, ExportPath, ChartInfo.Basic.Charter));
    }

    [RelayCommand]
    void NormalizeFileNames()
    {
        if(_folderFiles?.Normalize()==true)
            OnPropertyChanged(nameof(FolderFiles));
    }

    partial void OnExportPathChanged(string value)
    {
        if (!Directory.Exists(value))
            return;
        SetProperty(ref _folderFiles, new FolderFileSet(value), nameof(FolderFiles));
    }

    private class FolderFileSet : IEnumerable<string>, INotifyCollectionChanged
    {
        private readonly string _dir;

        private string? _music;
        private string? _pv;
        private string? _cvr;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        private static readonly NotifyCollectionChangedEventArgs s_collectionReset = new(NotifyCollectionChangedAction.Reset);

        public string? Music => _music;
        public string? Preview => _pv;
        public string?[] Charts { get; } = new string?[4];
        public string? Cover => _cvr;

        public FolderFileSet(string directory)
        {
            _dir = directory;

            foreach (string filePath in Directory.GetFiles(_dir)) {
                string file = Path.GetFileName(filePath);

                if (file.EndsWith(".mp3")) {
                    if (_music == null) {
                        _music = file;
                    }
                    else {
                        _pv = file;
                        if (new FileInfo(Path.Combine(_dir, _music)).Length < new FileInfo(Path.Combine(_dir, _pv)).Length)
                            (_music, _pv) = (_pv, _music);
                    }
                }
                else if (ValidateChartName(file, "easy", "Easy"))
                    Charts[0] ??= file;
                else if (ValidateChartName(file, "normal", "Normal"))
                    Charts[1] ??= file;
                else if (ValidateChartName(file, "hard", "Hard"))
                    Charts[2] ??= file;
                else if (ValidateChartName(file, "extra", "Extra", "ultra", "Ultra"))
                    Charts[3] ??= file;
                else if (file.EndsWith(".png"))
                    _cvr ??= file;
            }

            static bool ValidateChartName(string fileName, params string[] difficulties)
                => difficulties.Any(diff => fileName.EndsWith($".{diff}.json") || fileName == $"{diff}.json");
        }

        public bool Normalize()
        {
            bool somethingChanged = false;

            Normalize(ref _music, "Music.mp3");
            Normalize(ref _pv, "Preview.mp3");
            Normalize(ref Charts[0], "Easy.json");
            Normalize(ref Charts[1], "Normal.json");
            Normalize(ref Charts[2], "Hard.json");
            Normalize(ref Charts[3], "Extra.json");
            Normalize(ref _cvr, "Cover.png");
            if (_cvr == null) {
                _cvr = "Cover.png";
                File.Create(Path.Combine(_dir, _cvr)).Close();
                somethingChanged = true;
            }

            if (somethingChanged)
                CollectionChanged?.Invoke(this, s_collectionReset);
            return somethingChanged;

            void Normalize(ref string? field, string value)
            {
                if (field == null || field == value)
                    return;

                File.Move(Path.Combine(_dir, field), Path.Combine(_dir, value));
                field = value;
                somethingChanged = true;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (Music != null) yield return $"mu: {Music}";
            if (Preview != null) yield return $"pv: {Preview}";
            for (int i = 0; i < 4; i++)
                if (Charts[i] != null)
                    yield return $"{((ChartDifficulty)i).ShortNameLowerCase()}: {Charts[i]}";
            if (Cover != null) yield return $"cv: {Cover}";
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
