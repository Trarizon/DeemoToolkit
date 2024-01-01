using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Trarizon.Library.Collections.Extensions;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Entities;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator;
internal partial class MainWindowViewModel : ObservableObject
{
    private FolderFiles? _folderFiles;

    [ObservableProperty] string _exportPath;

    public ChartInfo ChartInfo { get; set; } = new();

    public IEnumerable<string> ProjectFiles => _folderFiles ?? Enumerable.Empty<string>();

    public MainWindowViewModel()
    {
        (_exportPath, ChartInfo) = Configuration.Load();
    }

    [RelayCommand]
    void ExportIni()
    {
        File.WriteAllText(
            Path.Combine(ExportPath, $"{ChartInfo.MusicName.Replace(Path.GetInvalidFileNameChars(), '_')}.ini"),
            ChartInfo.GetIni());
    }

    [RelayCommand]
    void ExportTxt()
    {
        var texts = ChartInfo.GetTxts();
        for (int i = 0; i < 4; i++) {
            if (texts[i] is null)
                continue;

            File.WriteAllText(
                Path.Combine(ExportPath, $"{ChartInfo.MusicName.Replace(Path.GetInvalidFileNameChars(), '_')}.{((ChartDifficulty)i).ToLowerCaseShortName()}.txt"),
                texts[i]);
        }
    }

    [RelayCommand]
    void SaveConfiguration()
    {
        Configuration.Save(ChartInfo, ExportPath);
    }

    [RelayCommand]
    void NormalizeFileNames()
    {
        if (_folderFiles?.Normalize() == true)
            OnPropertyChanged(nameof(ProjectFiles));
    }

    partial void OnExportPathChanged(string value)
    {
        if (!Directory.Exists(value))
            return;
        SetProperty(ref _folderFiles, new FolderFiles(value), nameof(ProjectFiles));
    }

    private class FolderFiles : IEnumerable<string>, INotifyCollectionChanged
    {
        private readonly string _dir;

        private string? _music;
        private string? _pv;
        private string? _cvr;

        private ChartAdaptedArray<string?> _charts = [];

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        private static readonly NotifyCollectionChangedEventArgs CollectionResetEvArgs = new(NotifyCollectionChangedAction.Reset);

        public FolderFiles(string directory)
        {
            _dir = directory;

            var groups = Directory.GetFiles(_dir).GroupBy(static file =>
            {
                if (file.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                    return 4;
                else if (file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    return 5;
                else if (file.EndsWith("easy.json", StringComparison.OrdinalIgnoreCase))
                    return 0;
                else if (file.EndsWith("normal.json", StringComparison.OrdinalIgnoreCase))
                    return 1;
                else if (file.EndsWith("hard.json", StringComparison.OrdinalIgnoreCase))
                    return 2;
                else if (file.EndsWith("extra.json", StringComparison.OrdinalIgnoreCase))
                    return 3;
                else if (file.EndsWith("ultra.json", StringComparison.OrdinalIgnoreCase))
                    return 3;
                return -1;
            });

            foreach (var group in groups) {
                var key = group.Key;
                switch (key) {
                    case >= 0 and < 4: // Charts
                        _charts[key] = group.First();
                        break;
                    case 4: // Music
                        if (!group.Any())
                            break;

                        var (min, max) = group.Select(f => new FileInfo(f)).MinMaxBy(fi => fi.Length);
                        _music = max.FullName;
                        if (min != max) // If min == max, means there's only one mp3
                            _pv = min.FullName;
                        break;
                    case 5: // Cover
                        _cvr = group.First();
                        break;
                }
            }
        }

        public bool Normalize()
        {
            bool changed =
                Normalize(ref _music, "Music.mp3")
                | Normalize(ref _pv, "Preview.mp3")
                | Normalize(ref _charts[0], "Easy.json")
                | Normalize(ref _charts[1], "Normal.json")
                | Normalize(ref _charts[2], "Hard.json")
                | Normalize(ref _charts[3], "Extra.json")
                | Normalize(ref _cvr, "Cover.png");
            if (_cvr == null) {
                _cvr = Path.Combine(_dir, "Cover.png");
                File.Create(_cvr).Close();
                changed = true;
            };

            if (changed)
                CollectionChanged?.Invoke(this, CollectionResetEvArgs);
            return changed;

            bool Normalize(ref string? field, string fileName)
            {
                if (field == null)
                    return false;

                var file = Path.Combine(_dir, fileName);
                if (field == file)
                    return false;

                File.Move(field, file);
                field = file;
                return true;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (_music != null)
                yield return $"mu: {_music[(_dir.Length + 1)..]}";
            if (_pv != null) yield return $"pv: {_pv[(_dir.Length + 1)..]}";
            for (int i = 0; i < 4; i++)
                if (_charts[i] != null)
                    yield return $"{((ChartDifficulty)i).ToLowerCaseShortName()}: {_charts[i]![(_dir.Length + 1)..]}";
            if (_cvr != null) yield return $"cv:{_cvr[(_dir.Length + 1)..]}";
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
