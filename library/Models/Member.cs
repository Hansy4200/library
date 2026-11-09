
using System.ComponentModel.DataAnnotations;

namespace library.Models;

// One Member object holds one row of lending.member.
public class Member
{
    public long MemberId { get; set; }

    [StringLength(80,
        ErrorMessage = "The name can be at most 80 characters.")]
    [RegularExpression(
        @"^[A-Za-zÑñ .'-]+, [A-Za-zÑñ .'-]+$",
        ErrorMessage = "Write the name as Lastname, Firstname.")]
    public string FullName { get; set; } = "";

    [StringLength(120,
        ErrorMessage = "The email can be at most 120 characters.")]
    [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$",
        ErrorMessage = "Enter an email such as ana@ubaguio.edu.")]
    public string? Email { get; set; }
    public DateOnly JoinDate { get; set; }
    public string MemberType { get; set; } = "";
   
}
