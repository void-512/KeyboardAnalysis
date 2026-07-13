using KeyboardAnalysis.Domain.Models;

namespace KeyboardAnalysis.Application.Interfaces;

/// <summary>Persists and queries locally captured keyboard activity.</summary>
public interface IKeyEventRepository
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task AddRangeAsync(IReadOnlyCollection<KeyEvent> keyEvents, CancellationToken cancellationToken = default);
}
