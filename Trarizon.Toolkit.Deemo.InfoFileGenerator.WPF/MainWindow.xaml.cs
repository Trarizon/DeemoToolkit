using System;
using System.IO;
using System.Windows;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.ViewModels;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowVM();
    }

    private void PreviewDragOverExportFolder(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
            e.Effects = DragDropEffects.Link;
            e.Handled = true; ;
        }
    }

    private void PreviewDropExportFolder(object sender, DragEventArgs e)
    {
        var value = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0)?.ToString();
        if (value == null)
            return;

        MainWindowVM vm = (MainWindowVM)DataContext;

        // is directory
        if (Directory.Exists(value)) {
            vm.ExportPath = value;
            return;
        }

        // is file
        value = Path.GetDirectoryName(value);
        if (Directory.Exists(value)) {
            vm.ExportPath = value;
            return;
        }
    }
}
