
using library.Models;
using Npgsql;

namespace library.Data;

public class MemberRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public MemberRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // READ: Copy one database row into a Member.
    private static Member ReadMember(NpgsqlDataReader reader)
    {
        return new Member
        {
            MemberId = reader.GetInt64(0),
            FullName = reader.GetString(1),
            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
            JoinDate = reader.GetFieldValue<DateOnly>(3),
            MemberType = reader.GetString(4)
        };
    }

    // READ: Get all members and count their currently borrowed books.

    public async Task<List<Member>> GetAllAsync()
    {
        const string sql =
            "SELECT m.member_id, m.full_name, m.email, m.join_date, " +
            "m.member_type, COUNT(l.loan_id) " +
            "FROM lending.member m " +
            "LEFT JOIN lending.loan l ON l.member_id = m.member_id " +
            "AND l.returned_on IS NULL " +
            "GROUP BY m.member_id " +
            "ORDER BY m.full_name;";

        var members = new List<Member>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var member = ReadMember(reader);

            // Get the number of books currently borrowed.
            member.OpenLoans = reader.GetInt64(5);

            members.Add(member);
        }

        return members;
    }


    // CREATE: Add a new member.
    public async Task AddAsync(Member member)
    {
        const string sql =
            "INSERT INTO lending.member " +
            "(full_name, email, join_date, member_type) " +
            "VALUES (@full_name, @email, @join_date, @member_type);";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("full_name", member.FullName);
        command.Parameters.AddWithValue(
            "email", (object?)member.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("join_date", member.JoinDate);
        command.Parameters.AddWithValue("member_type", member.MemberType);

        await command.ExecuteNonQueryAsync();
    }

    // READ: Get one member using their ID.
    public async Task<Member?> GetByIdAsync(long id)
    {
        const string sql =
            "SELECT member_id, full_name, email, join_date, member_type " +
            "FROM lending.member WHERE member_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? ReadMember(reader) : null;
    }

    // UPDATE: Edit an existing member.
    public async Task UpdateAsync(Member member)
    {
        const string sql =
            "UPDATE lending.member " +
            "SET full_name = @full_name, " +
            "email = @email, " +
            "join_date = @join_date, " +
            "member_type = @member_type " +
            "WHERE member_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("full_name", member.FullName);
        command.Parameters.AddWithValue(
            "email", (object?)member.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("join_date", member.JoinDate);
        command.Parameters.AddWithValue("member_type", member.MemberType);
        command.Parameters.AddWithValue("id", member.MemberId);

        await command.ExecuteNonQueryAsync();
    }

    // DELETE: Remove a member.
    public async Task DeleteAsync(long id)
    {
        const string sql =
            "DELETE FROM lending.member WHERE member_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}
