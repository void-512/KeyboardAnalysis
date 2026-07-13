using Microsoft.UI.Xaml.Controls;

namespace KeyboardAnalysis.Presentation.Controls;

public sealed partial class DashboardCard : UserControl
{
    public DashboardCard() => InitializeComponent();

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(DashboardCard), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(DashboardCard), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty CardContentProperty = DependencyProperty.Register(nameof(CardContent), typeof(UIElement), typeof(DashboardCard), new PropertyMetadata(null));

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    public UIElement? CardContent { get => (UIElement?)GetValue(CardContentProperty); set => SetValue(CardContentProperty, value); }
}
