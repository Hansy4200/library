namespace library.Models;

// One Member object holds one row of lending.member.
public class Member
{
    public long MemberId { get; set; }            // member_id    BIGINT, the primary key
    public string FullName { get; set; } = "";    // full_name    VARCHAR(80) NOT NULL
    public string? Email { get; set; }            // email        VARCHAR(120), may be NULL, must be unique
    public DateOnly JoinDate { get; set; }        // join_date    DATE NOT NULL
    public string MemberType { get; set; } = "Regular"; // member_type  VARCHAR(10) NOT NULL: Regular, Student or Faculty

}
