namespace KeyboardAnalysis.Domain.Models;

/// <summary>One completed keyboard interaction captured on the local device.</summary>
public sealed record KeyEvent(
    DateTimeOffset Timestamp,
    int KeyCode,
    TimeSpan Duration,
    string ActiveApplication);
