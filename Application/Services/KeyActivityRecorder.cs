using KeyboardAnalysis.Application.Interfaces;
using KeyboardAnalysis.Domain.Models;

namespace KeyboardAnalysis.Application.Services;

/// <summary>
/// Buffers captured activity in memory and saves a batch after 20 seconds without new activity.
/// </summary>
public sealed class KeyActivityRecorder
{
    private static readonly TimeSpan InactivityFlushDelay = TimeSpan.FromSeconds(20);
    private readonly IKeyboardCaptureService _keyboardCapture;
    private readonly IKeyEventRepository _repository;
    private readonly ConcurrentQueue<KeyEvent> _pendingEvents = new();
    private readonly SemaphoreSlim _activitySignal = new(0);
    private readonly object _lifecycleLock = new();
    private CancellationTokenSource? _flushCancellation;
    private Task? _flushWorker;
    private long _lastActivityUtcTicks;

    public KeyActivityRecorder(IKeyboardCaptureService keyboardCapture, IKeyEventRepository repository)
    {
        _keyboardCapture = keyboardCapture;
        _repository = repository;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _repository.InitializeAsync(cancellationToken);
        lock (_lifecycleLock)
        {
            if (_flushWorker is not null)
            {
                return;
            }

            _flushCancellation = new CancellationTokenSource();
            _flushWorker = FlushAfterInactivityAsync(_flushCancellation.Token);
            _keyboardCapture.KeyEventCaptured += OnKeyEventCaptured;
        }

        try
        {
            await _keyboardCapture.StartAsync(cancellationToken);
        }
        catch
        {
            await StopAsync(cancellationToken);
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _keyboardCapture.KeyEventCaptured -= OnKeyEventCaptured;
        await _keyboardCapture.StopAsync(cancellationToken);

        Task? worker;
        lock (_lifecycleLock)
        {
            _flushCancellation?.Cancel();
            worker = _flushWorker;
            _flushWorker = null;
            _flushCancellation?.Dispose();
            _flushCancellation = null;
        }

        if (worker is not null)
        {
            try
            {
                await worker;
            }
            catch (OperationCanceledException)
            {
                // Expected when capture is stopped before the inactivity interval elapses.
            }
        }

        await FlushPendingEventsAsync(cancellationToken);
    }

    private void OnKeyEventCaptured(object? sender, KeyEvent keyEvent)
    {
        _pendingEvents.Enqueue(keyEvent);
        Interlocked.Exchange(ref _lastActivityUtcTicks, DateTime.UtcNow.Ticks);
        _activitySignal.Release();
    }

    private async Task FlushAfterInactivityAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            await _activitySignal.WaitAsync(cancellationToken);

            while (true)
            {
                var activitySnapshot = Interlocked.Read(ref _lastActivityUtcTicks);
                await Task.Delay(InactivityFlushDelay, cancellationToken);

                if (activitySnapshot == Interlocked.Read(ref _lastActivityUtcTicks))
                {
                    await FlushPendingEventsAsync(cancellationToken);
                    while (_activitySignal.Wait(0)) { }
                    break;
                }
            }
        }
    }

    private async Task FlushPendingEventsAsync(CancellationToken cancellationToken)
    {
        var batch = new List<KeyEvent>();
        while (_pendingEvents.TryDequeue(out var keyEvent))
        {
            batch.Add(keyEvent);
        }

        if (batch.Count > 0)
        {
            await _repository.AddRangeAsync(batch, cancellationToken);
        }
    }
}
