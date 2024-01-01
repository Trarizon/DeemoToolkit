using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Trarizon.Toolkit.Deemo.InfoFileGenerator.Controls;
public sealed partial class PropertyTextBox : UserControl
{
    public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(PropertyTextBox), null);
    public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register(nameof(PropertyValue), typeof(string), typeof(PropertyTextBox), new(null));

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

    public PropertyTextBox()
    {
        this.InitializeComponent();
    }
}
