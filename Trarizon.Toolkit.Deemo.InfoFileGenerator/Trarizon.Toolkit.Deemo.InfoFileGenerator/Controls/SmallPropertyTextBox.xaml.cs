using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Controls;
internal sealed partial class SmallPropertyTextBox : UserControl
{
    public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(SmallPropertyTextBox), null);
    public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register(nameof(PropertyValue), typeof(string), typeof(SmallPropertyTextBox), new(null));

    public string PropertyName
    {
        get => (string)GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    public string PropertyValue
    {
        get => (string)GetValue(PropertyValueProperty);
        set => SetValue(PropertyValueProperty, value);
    }

    public SmallPropertyTextBox()
    {
        this.InitializeComponent();
    }
}
