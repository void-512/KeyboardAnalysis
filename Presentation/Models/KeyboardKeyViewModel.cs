namespace KeyboardAnalysis.Presentation.Models;

/// <summary>Display state for one independently rendered keyboard key.</summary>
public sealed class KeyboardKeyViewModel
{
    public KeyboardKeyViewModel(string label, double width, int pressCount)
    {
        Label = label;
        Width = width;
        PressCount = pressCount;
        HeatBrush = CreateHeatBrush(pressCount);
    }

    public string Label { get; }
    public double Width { get; }
    public int PressCount { get; }
    public string Tooltip => $"{Label}: {PressCount:N0} presses";
    public SolidColorBrush HeatBrush { get; }

    private static SolidColorBrush CreateHeatBrush(int count)
    {
        var intensity = Math.Clamp(count / 320d, 0d, 1d);
        var red = (byte)(238 - (intensity * 38));
        var green = (byte)(246 - (intensity * 122));
        var blue = (byte)(255 - (intensity * 78));
        return new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, red, green, blue));
    }
}

public sealed class KeyboardRowViewModel
{
    public KeyboardRowViewModel(IEnumerable<KeyboardKeyViewModel> keys) => Keys = keys.ToList();
    public IReadOnlyList<KeyboardKeyViewModel> Keys { get; }
}
