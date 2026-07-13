using KeyboardAnalysis.Application.Interfaces;
using KeyboardAnalysis.Domain.Models;

namespace KeyboardAnalysis.Infrastructure.Persistence;

/// <summary>Boundary for the future SQLite database schema and commands.</summary>
public sealed class SqliteKeyEventRepository : IKeyEventRepository
{
    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Create the local database and KeyEvents table.
        return Task.CompletedTask;
    }

    public Task AddAsync(KeyEvent keyEvent, CancellationToken cancellationToken = default)
    {
        // TODO: Insert Timestamp, KeyCode, Duration, and ActiveApplication into SQLite.
        return Task.CompletedTask;
    }
}
