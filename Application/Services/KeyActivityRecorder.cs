using KeyboardAnalysis.Application.Interfaces;
using KeyboardAnalysis.Domain.Models;

namespace KeyboardAnalysis.Application.Services;

/// <summary>Coordinates capture events and their durable local storage.</summary>
public sealed class KeyActivityRecorder
{
    private readonly IKeyboardCaptureService _keyboardCapture;
    private readonly IKeyEventRepository _repository;

    public KeyActivityRecorder(IKeyboardCaptureService keyboardCapture, IKeyEventRepository repository)
    {
        _keyboardCapture = keyboardCapture;
        _repository = repository;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _repository.InitializeAsync(cancellationToken);
        _keyboardCapture.KeyEventCaptured += OnKeyEventCaptured;
        await _keyboardCapture.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _keyboardCapture.KeyEventCaptured -= OnKeyEventCaptured;
        await _keyboardCapture.StopAsync(cancellationToken);
    }

    private async void OnKeyEventCaptured(object? sender, KeyEvent keyEvent)
    {
        // A later iteration can add batching, retries, and diagnostic logging here.
        await _repository.AddAsync(keyEvent);
    }
}
