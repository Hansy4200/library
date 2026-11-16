
namespace library.Models;

// One row of the Loans page.
public class LoanView
{
    public long LoanId { get; set; }
    public long MemberId { get; set; }
    public string MemberName { get; set; } = "";
    public string BookTitle { get; set; } = "";
    public DateOnly LoanedOn { get; set; }
    public DateOnly? ReturnedOn { get; set; }
    public int DaysAllowed { get; set; }
    public string Status { get; set; } = "";
}
