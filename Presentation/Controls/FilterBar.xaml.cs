using KeyboardAnalysis.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace KeyboardAnalysis.Presentation.Controls;

public sealed partial class FilterBar : UserControl
{
    public FilterBar() => InitializeComponent();
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(DashboardViewModel), typeof(FilterBar), new PropertyMetadata(null));
    public DashboardViewModel? ViewModel { get => (DashboardViewModel?)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

    private void OnMultiSelectChanged(object sender, SelectionChangedEventArgs e) => ViewModel?.RefreshMockData();
}
