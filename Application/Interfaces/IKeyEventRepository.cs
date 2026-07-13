using KeyboardAnalysis.Domain.Models;

namespace KeyboardAnalysis.Application.Interfaces;

/// <summary>Persists and queries locally captured keyboard activity.</summary>
public interface IKeyEventRepository
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task AddAsync(KeyEvent keyEvent, CancellationToken cancellationToken = default);
}
