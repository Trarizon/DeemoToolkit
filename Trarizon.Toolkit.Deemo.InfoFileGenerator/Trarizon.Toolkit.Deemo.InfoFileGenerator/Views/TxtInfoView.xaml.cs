using Microsoft.UI.Xaml.Controls;
using System.Runtime.CompilerServices;
using Trarizon.Toolkit.Deemo.InfoFileGenerator.ViewModels;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Views;
internal sealed partial class TxtInfoView : UserControl
{
    public TxtInfoViewModel ViewModel => Unsafe.As<TxtInfoViewModel>(DataContext);

    public TxtInfoView()
    {
        this.InitializeComponent();
    }
}
