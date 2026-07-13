using KeyboardAnalysis.Application.Interfaces;
using KeyboardAnalysis.Domain.Models;

namespace KeyboardAnalysis.Infrastructure.Persistence;

/// <summary>Stores captured keyboard activity in a local SQLite database.</summary>
public sealed class SqliteKeyEventRepository : IKeyEventRepository
{
    private readonly string _connectionString;

    public SqliteKeyEventRepository(string? databasePath = null)
    {
        databasePath ??= Path.Combine(
            AppContext.BaseDirectory,
            "keyboard-analysis.db");
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
        _connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString();
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS KeyEvents (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TimestampUtc TEXT NOT NULL,
                KeyCode INTEGER NOT NULL,
                DurationMilliseconds REAL NOT NULL,
                ActiveApplication TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS IX_KeyEvents_TimestampUtc ON KeyEvents (TimestampUtc);
            """;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IReadOnlyCollection<KeyEvent> keyEvents, CancellationToken cancellationToken = default)
    {
        if (keyEvents.Count == 0)
        {
            return;
        }

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.Transaction = (SqliteTransaction)transaction;
        command.CommandText = """
            INSERT INTO KeyEvents (TimestampUtc, KeyCode, DurationMilliseconds, ActiveApplication)
            VALUES ($timestampUtc, $keyCode, $durationMilliseconds, $activeApplication);
            """;
        var timestamp = command.Parameters.Add("$timestampUtc", SqliteType.Text);
        var keyCode = command.Parameters.Add("$keyCode", SqliteType.Integer);
        var duration = command.Parameters.Add("$durationMilliseconds", SqliteType.Real);
        var activeApplication = command.Parameters.Add("$activeApplication", SqliteType.Text);

        foreach (var keyEvent in keyEvents)
        {
            timestamp.Value = keyEvent.Timestamp.UtcDateTime.ToString("O");
            keyCode.Value = keyEvent.KeyCode;
            duration.Value = keyEvent.Duration.TotalMilliseconds;
            activeApplication.Value = keyEvent.ActiveApplication;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }
}
