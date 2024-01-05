using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator;
internal sealed partial class MainWindow : Window
{
    internal MainWindowViewModel ViewModel { get; }

    private readonly IniInfoViewModel _iniInfoViewModel;
    private readonly TxtInfoViewModel _txtInfoViewModel;

    private readonly float _factor;

    private int SizeToPixel(double value)
    {
        return (int)(value * _factor);
    }

    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel();
        _iniInfoViewModel = new(ViewModel.ChartInfo);
        _txtInfoViewModel = new(ViewModel.ChartInfo);
        _factor = PInvoke.GetDpiForWindow(this) / 96f;

        AppWindow.ResizeClient(new(SizeToPixel(contentGrid.ActualWidth), SizeToPixel(contentGrid.ActualHeight)));

        ProjectFilesUpdate();
    }

    // WinUI 3 Bug
    // in file: MainWindow.g.cs
    // in method: CollectionChanged_ViewModel_ProjectFiles
    private void ProjectFilesUpdate()
    {
        ViewModel.PropertyChanging += (s, e) =>
        {
            MainWindowViewModel vm = Unsafe.As<MainWindowViewModel>(s!);
            if (e.PropertyName == nameof(MainWindowViewModel.ProjectFiles)) {
                if (vm.ProjectFiles != null)
                    vm.ProjectFiles.CollectionChanged -= ProjectFiles_CollectionChanged;
            }
        };
        ViewModel.PropertyChanged += (s, e) =>
        {
            MainWindowViewModel vm = Unsafe.As<MainWindowViewModel>(s!);
            if (e.PropertyName == nameof(MainWindowViewModel.ProjectFiles)) {
                if (vm.ProjectFiles != null)
                    vm.ProjectFiles.CollectionChanged += ProjectFiles_CollectionChanged;
            }
        };

        void ProjectFiles_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            projectFiles_ListView.ItemsSource = null;
            projectFiles_ListView.ItemsSource = ViewModel.ProjectFiles;
        }
    }

    private void ListView_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Link;
    }

    private async void ListView_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems)) {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 0)
                return;
            var path = items[0] switch {
                StorageFile sfile => Path.GetDirectoryName(sfile.Path)!,
                StorageFolder sfdr => sfdr.Path,
                _ => null,
            };
            if (path is not null)
                ViewModel.UpdateProjectFiles(path);
        }
    }

    private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        AppWindow.ResizeClient(new(SizeToPixel(e.NewSize.Width), SizeToPixel(e.NewSize.Height)));
    }
}
