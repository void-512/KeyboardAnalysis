using KeyboardAnalysis.Presentation.Models;
using Microsoft.UI.Xaml.Controls;

namespace KeyboardAnalysis.Presentation.Controls;

public sealed partial class RawDataTable : UserControl
{
    public RawDataTable() => InitializeComponent();
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IEnumerable<RawStatisticRow>), typeof(RawDataTable), new PropertyMetadata(null));
    public IEnumerable<RawStatisticRow>? Items { get => (IEnumerable<RawStatisticRow>?)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }
}
