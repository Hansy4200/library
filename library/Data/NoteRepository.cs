
using library.Models;
using Npgsql;

namespace library.Data;

public class NoteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NoteRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Get all notes
    public async Task<List<Note>> GetAllAsync()
    {
        var notes = new List<Note>();

        const string sql = """
            SELECT note_id, title, body, created_on
            FROM lending.note
            ORDER BY note_id;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            notes.Add(new Note
            {
                NoteId = reader.GetInt64(0),
                Title = reader.GetString(1),
                Body = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedOn = reader.GetDateTime(3)
            });
        }

        return notes;
    }

    // Get one note by ID
    public async Task<Note?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT note_id, title, body, created_on
            FROM lending.note
            WHERE note_id = @id;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Note
        {
            NoteId = reader.GetInt64(0),
            Title = reader.GetString(1),
            Body = reader.IsDBNull(2) ? null : reader.GetString(2),
            CreatedOn = reader.GetDateTime(3)
        };
    }

    // Add a note; PostgreSQL generates the ID and creation date
    public async Task AddAsync(Note note)
    {
        const string sql = """
            INSERT INTO lending.note (title, body)
            VALUES (@title, @body);
            """;

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue(
            "body", (object?)note.Body ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    // Update a note
    public async Task UpdateAsync(Note note)
    {
        const string sql = """
            UPDATE lending.note
            SET title = @title,
                body = @body,
                created_on = @created_on
            WHERE note_id = @id;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("id", note.NoteId);
        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue(
            "body", (object?)note.Body ?? DBNull.Value);
        command.Parameters.AddWithValue("created_on", note.CreatedOn.Date);

        await command.ExecuteNonQueryAsync();
    }

    // Delete a note
    public async Task DeleteAsync(long id)
    {
        const string sql = """
            DELETE FROM lending.note
            WHERE note_id = @id;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}
