using KeyboardAnalysis.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace KeyboardAnalysis.Presentation.Pages;

public sealed partial class DashboardPage : UserControl
{
    public DashboardPage() => InitializeComponent();
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(DashboardViewModel), typeof(DashboardPage), new PropertyMetadata(null));
    public DashboardViewModel? ViewModel { get => (DashboardViewModel?)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
}
