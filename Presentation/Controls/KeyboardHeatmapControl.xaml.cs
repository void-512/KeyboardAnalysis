using KeyboardAnalysis.Presentation.Models;
using Microsoft.UI.Xaml.Controls;

namespace KeyboardAnalysis.Presentation.Controls;

public sealed partial class KeyboardHeatmapControl : UserControl
{
    public KeyboardHeatmapControl() => InitializeComponent();

    public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(nameof(Rows), typeof(IReadOnlyList<KeyboardRowViewModel>), typeof(KeyboardHeatmapControl), new PropertyMetadata(null));
    public IReadOnlyList<KeyboardRowViewModel>? Rows { get => (IReadOnlyList<KeyboardRowViewModel>?)GetValue(RowsProperty); set => SetValue(RowsProperty, value); }
}
