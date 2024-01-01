using Microsoft.UI.Xaml;
using System;
using System.IO;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.Utilities;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator;
internal sealed partial class MainWindow : Window
{
    internal MainWindowViewModel ViewModel { get; }

    private IniInfoViewModel _iniInfoViewModel;
    private TxtInfoViewModel _txtInfoViewModel;

    private float _factor;

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
                ViewModel.ExportPath = path;
        }
    }

    private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        AppWindow.ResizeClient(new(SizeToPixel(e.NewSize.Width), SizeToPixel(e.NewSize.Height)));
    }
}
