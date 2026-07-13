using Microsoft.UI.Xaml.Data;

namespace KeyboardAnalysis.Presentation.Models;

[Bindable]
public sealed class RawStatisticRow
{
    public RawStatisticRow(string key, int pressCount)
    {
        Key = key;
        PressCount = pressCount;
    }

    public string Key { get; }
    public int PressCount { get; }
}
