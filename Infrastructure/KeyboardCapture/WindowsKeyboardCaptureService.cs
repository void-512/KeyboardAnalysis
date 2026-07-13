using KeyboardAnalysis.Application.Interfaces;
using KeyboardAnalysis.Domain.Models;

namespace KeyboardAnalysis.Infrastructure.KeyboardCapture;

/// <summary>Boundary for the future Windows low-level keyboard-hook implementation.</summary>
public sealed class WindowsKeyboardCaptureService : IKeyboardCaptureService
{
    public event EventHandler<KeyEvent>? KeyEventCaptured;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Register the keyboard hook and emit completed key events.
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Unregister the keyboard hook.
        return Task.CompletedTask;
    }

    private void PublishKeyEvent(KeyEvent keyEvent) => KeyEventCaptured?.Invoke(this, keyEvent);
}
