
using library.Models;
using Npgsql;

namespace library.Data;

public class LoanRepository
{
    public const int MaxOpenLoans = 3;

    private readonly NpgsqlDataSource _dataSource;

    public LoanRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    private static LoanView ReadLoan(NpgsqlDataReader reader)
    {
        return new LoanView
        {
            LoanId = reader.GetInt64(0),
            MemberId = reader.GetInt64(1),
            MemberName = reader.GetString(2),
            BookTitle = reader.GetString(3),
            LoanedOn = reader.GetFieldValue<DateOnly>(4),
            ReturnedOn = reader.IsDBNull(5)
                ? null
                : reader.GetFieldValue<DateOnly>(5),
            DaysAllowed = reader.GetInt32(6),
            Status = reader.GetString(7)
        };
    }


    public async Task<List<LoanView>> GetAllAsync()
    {
        const string sql =
            "SELECT l.loan_id, l.member_id, m.full_name, b.title, " +
            "l.loaned_on, l.returned_on, l.days_allowed, " +
            "CASE WHEN l.returned_on IS NOT NULL THEN 'Returned' " +
            "WHEN l.loaned_on + l.days_allowed < CURRENT_DATE THEN 'Overdue' " +
            "ELSE 'Out' END " +
            "FROM lending.loan l " +
            "JOIN lending.member m ON l.member_id = m.member_id " +
            "JOIN lending.book b ON l.book_id = b.book_id " +
            "ORDER BY l.loaned_on DESC, l.loan_id DESC;";

        var loans = new List<LoanView>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            loans.Add(ReadLoan(reader));
        }

        return loans;
    }
}
