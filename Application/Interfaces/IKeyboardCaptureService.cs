using KeyboardAnalysis.Domain.Models;

namespace KeyboardAnalysis.Application.Interfaces;

/// <summary>Captures keyboard activity from the operating system.</summary>
public interface IKeyboardCaptureService
{
    event EventHandler<KeyEvent>? KeyEventCaptured;
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
