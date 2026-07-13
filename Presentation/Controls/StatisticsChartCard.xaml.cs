using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Kernel.Sketches;
using Microsoft.UI.Xaml.Controls;

namespace KeyboardAnalysis.Presentation.Controls;

public sealed partial class StatisticsChartCard : UserControl
{
    public StatisticsChartCard() => InitializeComponent();
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(StatisticsChartCard), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(StatisticsChartCard), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(nameof(Series), typeof(IEnumerable<ISeries>), typeof(StatisticsChartCard), new PropertyMetadata(null));
    public static readonly DependencyProperty XAxesProperty = DependencyProperty.Register(nameof(XAxes), typeof(IEnumerable<ICartesianAxis>), typeof(StatisticsChartCard), new PropertyMetadata(null));
    public static readonly DependencyProperty YAxesProperty = DependencyProperty.Register(nameof(YAxes), typeof(IEnumerable<ICartesianAxis>), typeof(StatisticsChartCard), new PropertyMetadata(null));
    public static readonly DependencyProperty LegendPositionProperty = DependencyProperty.Register(nameof(LegendPosition), typeof(LegendPosition), typeof(StatisticsChartCard), new PropertyMetadata(LegendPosition.Hidden));
    public static readonly DependencyProperty TooltipPositionProperty = DependencyProperty.Register(nameof(TooltipPosition), typeof(TooltipPosition), typeof(StatisticsChartCard), new PropertyMetadata(TooltipPosition.Top));
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    public IEnumerable<ISeries>? Series { get => (IEnumerable<ISeries>?)GetValue(SeriesProperty); set => SetValue(SeriesProperty, value); }
    public IEnumerable<ICartesianAxis>? XAxes { get => (IEnumerable<ICartesianAxis>?)GetValue(XAxesProperty); set => SetValue(XAxesProperty, value); }
    public IEnumerable<ICartesianAxis>? YAxes { get => (IEnumerable<ICartesianAxis>?)GetValue(YAxesProperty); set => SetValue(YAxesProperty, value); }
    public LegendPosition LegendPosition { get => (LegendPosition)GetValue(LegendPositionProperty); set => SetValue(LegendPositionProperty, value); }
    public TooltipPosition TooltipPosition { get => (TooltipPosition)GetValue(TooltipPositionProperty); set => SetValue(TooltipPositionProperty, value); }
}
