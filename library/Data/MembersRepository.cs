using library.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace library.Data;

// Every SQL statement about members lives in this one class.
public class MembersRepository
{
    private readonly NpgsqlDataSource _dataSource;

    // ASP.NET Core hands in the data source that Program.cs registered.
    public MembersRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Copies the current row of the reader into a Member.
    private static Members ReadMember(NpgsqlDataReader reader)
    {
        return new Members
        {
            MemberId = reader.GetInt64(0),
            Title = reader.GetString(1),
            Category = reader.IsDBNull(2) ? null : reader.GetString(2),
            Price = reader.IsDBNull(3) ? null : reader.GetDecimal(3)
        };
    }

    // READ: every member, sorted by name.
    public async Task<List<Members>> GetAllAsync()
    {
        const string sql = "SELECT member_id, title, category, price FROM lending.member ORDER BY title;";

        var members = new List<Members>();

        // The command borrows a connection; 'await using' gives it back when the method ends.
        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())        // once for each row
        {
            members.Add(ReadMember(reader));
        }

        return members;
    }

    // CREATE: insert a new member. The database chooses its member_id.
    public async Task AddAsync(Members member)
    {
        const string sql = "INSERT INTO lending.member (name, category, price) " +
                           "VALUES (@name, @category, @price);";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("name", member.Title);
        // A C# null must be sent as DBNull.Value, which is how a database NULL is written.
        command.Parameters.AddWithValue("category", (object?)member.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("price", (object?)member.Price ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();           // runs SQL that returns no rows
    }

    // READ: one member, or null when no member has that id.
    public async Task<Members?> GetByIdAsync(long id)
    {
        const string sql = "SELECT member_id, name, category, price FROM lending.member WHERE member_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);      // @id in the SQL receives this value

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadMember(reader) : null;
    }

    public async Task UpdateAsync(Members member)
    {
        const string sql = "UPDATE lending.member " +
                           "SET name = @name, category = @category, price = @price " +
                           "WHERE member_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("title", member.Title);
        command.Parameters.AddWithValue("category", (object?)member.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("price", (object?)member.Price ?? DBNull.Value);
        command.Parameters.AddWithValue("id", member.MemberId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(long id)
    {
        const string sql = "DELETE FROM lending.member WHERE member_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}