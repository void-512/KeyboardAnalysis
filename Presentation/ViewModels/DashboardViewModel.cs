using KeyboardAnalysis.Presentation.Models;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;

namespace KeyboardAnalysis.Presentation.ViewModels;

/// <summary>Dashboard presentation state using mock values until query services are introduced.</summary>
public sealed class DashboardViewModel : ViewModelBase
{
    private string _selectedTimeInterval = "Today";
    private string _selectedAggregation = "Hour";
    private IEnumerable<ISeries> _durationHistogramSeries = Array.Empty<ISeries>();
    private IEnumerable<ISeries> _pressCountSeries = Array.Empty<ISeries>();
    private IEnumerable<ISeries> _averageDurationSeries = Array.Empty<ISeries>();
    public DashboardViewModel()
    {
        TimeIntervals = new ObservableCollection<string>(new[]
        {
            "Today", "Yesterday", "Last 7 Days", "Last 30 Days", "Last 90 Days", "This Year", "Custom Range"
        });
        Aggregations = new ObservableCollection<string>(new[] { "Hour", "Day", "Week", "Month", "Year" });
        KeyOptions = new ObservableCollection<SelectableOption>(new[] { "A", "S", "D", "F", "Space", "Enter", "Shift", "Ctrl" }.Select(x => new SelectableOption(x)));
        ProgramOptions = new ObservableCollection<SelectableOption>(new[] { "Chrome", "Visual Studio", "Word", "Explorer", "Discord" }.Select(x => new SelectableOption(x)));
        KeyboardRows = BuildKeyboardRows();
        RawStatistics = new ObservableCollection<RawStatisticRow>(new[]
        {
            new RawStatisticRow("Space", 4_862), new RawStatisticRow("E", 3_910), new RawStatisticRow("A", 2_740),
            new RawStatisticRow("Enter", 1_182), new RawStatisticRow("Shift", 876), new RawStatisticRow("Ctrl", 421)
        });

        DurationHistogramXAxes = new[] { new Axis { Labels = new[] { "0–50", "50–100", "100–200", "200–400", "400–700", "700+" }, Name = "Milliseconds" } };
        DurationHistogramYAxes = new[] { new Axis { Name = "Frequency" } };
        TrendXAxes = new[] { new Axis { Labels = new[] { "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" }, Name = "Hour" } };
        PressCountYAxes = new[] { new Axis { Name = "Presses" } };
        AverageDurationYAxes = new[] { new Axis { Name = "Milliseconds" } };
        ResetFiltersCommand = new DelegateCommand(ResetFilters);
        RefreshMockData();
    }

    public ObservableCollection<string> TimeIntervals { get; }
    public ObservableCollection<string> Aggregations { get; }
    public ObservableCollection<SelectableOption> KeyOptions { get; }
    public ObservableCollection<SelectableOption> ProgramOptions { get; }
    public IReadOnlyList<KeyboardRowViewModel> KeyboardRows { get; }
    public ObservableCollection<RawStatisticRow> RawStatistics { get; }
    public IEnumerable<ISeries> DurationHistogramSeries
    {
        get => _durationHistogramSeries;
        private set { _durationHistogramSeries = value; OnPropertyChanged(); }
    }
    public IEnumerable<ICartesianAxis> DurationHistogramXAxes { get; }
    public IEnumerable<ICartesianAxis> DurationHistogramYAxes { get; }
    public IEnumerable<ISeries> PressCountSeries
    {
        get => _pressCountSeries;
        private set { _pressCountSeries = value; OnPropertyChanged(); }
    }
    public IEnumerable<ISeries> AverageDurationSeries
    {
        get => _averageDurationSeries;
        private set { _averageDurationSeries = value; OnPropertyChanged(); }
    }
    public IEnumerable<ICartesianAxis> TrendXAxes { get; }
    public IEnumerable<ICartesianAxis> PressCountYAxes { get; }
    public IEnumerable<ICartesianAxis> AverageDurationYAxes { get; }
    public LegendPosition ChartLegendPosition => LegendPosition.Hidden;
    public TooltipPosition ChartTooltipPosition => TooltipPosition.Top;
    public DelegateCommand ResetFiltersCommand { get; }

    public string SelectedTimeInterval
    {
        get => _selectedTimeInterval;
        set
        {
            if (_selectedTimeInterval == value) return;
            _selectedTimeInterval = value;
            OnPropertyChanged();
            RefreshMockData();
        }
    }

    public string SelectedAggregation
    {
        get => _selectedAggregation;
        set
        {
            if (_selectedAggregation == value) return;
            _selectedAggregation = value;
            OnPropertyChanged();
            RefreshMockData();
        }
    }

    public void RefreshMockData()
    {
        var intervalFactor = TimeIntervals.IndexOf(SelectedTimeInterval) + 1;
        var aggregationFactor = Aggregations.IndexOf(SelectedAggregation) + 1;
        var factor = Math.Max(1, intervalFactor + aggregationFactor);
        DurationHistogramSeries = new ISeries[] { new ColumnSeries<double> { Name = "Presses", Values = new double[] { 24, 96, 178, 120, 52, 19 }.Select(value => value * factor).ToArray() } };
        PressCountSeries = new ISeries[] { new LineSeries<double> { Name = "Presses", Values = new double[] { 74, 96, 122, 164, 145, 208, 190, 232, 214, 268, 251, 300 }.Select(value => value * factor).ToArray() } };
        AverageDurationSeries = new ISeries[] { new LineSeries<double> { Name = "Average", Values = new double[] { 96, 102, 88, 112, 109, 124, 116, 130, 126, 119, 113, 108 }.Select(value => value + aggregationFactor * 3).ToArray() } };
    }

    private void ResetFilters()
    {
        SelectedTimeInterval = "Today";
        SelectedAggregation = "Hour";
        RefreshMockData();
    }

    private static IReadOnlyList<KeyboardRowViewModel> BuildKeyboardRows()
    {
        var random = new Random(7);
        KeyboardKeyViewModel Key(string label, double width = 46) => new(label, width, random.Next(12, 350));
        return new[]
        {
            new KeyboardRowViewModel(new[] { Key("Esc"), Key("F1"), Key("F2"), Key("F3"), Key("F4"), Key("F5"), Key("F6"), Key("F7"), Key("F8"), Key("F9"), Key("F10"), Key("F11"), Key("F12") }),
            new KeyboardRowViewModel(new[] { Key("`"), Key("1"), Key("2"), Key("3"), Key("4"), Key("5"), Key("6"), Key("7"), Key("8"), Key("9"), Key("0"), Key("-"), Key("="), Key("Backspace", 92) }),
            new KeyboardRowViewModel(new[] { Key("Tab", 70), Key("Q"), Key("W"), Key("E"), Key("R"), Key("T"), Key("Y"), Key("U"), Key("I"), Key("O"), Key("P"), Key("["), Key("]"), Key("\\", 68) }),
            new KeyboardRowViewModel(new[] { Key("Caps", 82), Key("A"), Key("S"), Key("D"), Key("F"), Key("G"), Key("H"), Key("J"), Key("K"), Key("L"), Key(";"), Key("'"), Key("Enter", 94) }),
            new KeyboardRowViewModel(new[] { Key("Shift", 104), Key("Z"), Key("X"), Key("C"), Key("V"), Key("B"), Key("N"), Key("M"), Key(","), Key("."), Key("/"), Key("Shift", 116) }),
            new KeyboardRowViewModel(new[] { Key("Ctrl", 64), Key("Win", 58), Key("Alt", 58), Key("Space", 320), Key("Alt", 58), Key("Ctrl", 64) })
        };
    }
}
