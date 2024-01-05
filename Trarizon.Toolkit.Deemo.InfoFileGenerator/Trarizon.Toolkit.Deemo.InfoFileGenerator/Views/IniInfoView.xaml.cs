using Microsoft.UI.Xaml.Controls;
using System.Runtime.CompilerServices;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.ViewModels;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Views;
internal sealed partial class IniInfoView : UserControl
{
    public IniInfoViewModel ViewModel => Unsafe.As<IniInfoViewModel>(DataContext);

    public IniInfoView()
    {
        this.InitializeComponent();
    }
}
