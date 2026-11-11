
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace library.Controllers;

public class LoansController : Controller
{
    private readonly LoanRepository _loans;
    private readonly MemberRepository _members;
    private readonly BookRepository _books;

    public LoansController(
        LoanRepository loans,
        MemberRepository members,
        BookRepository books)
    {
        _loans = loans;
        _members = members;
        _books = books;
    }

    public async Task<IActionResult> Index()
    {
        var loans = await _loans.GetAllAsync();
        return View(loans);
    }
    private async Task FillDropdownsAsync()
    {
        ViewBag.Members = new SelectList(
            await _members.GetAllAsync(),
            "MemberId",
            "FullName");

        ViewBag.Books = new SelectList(
            await _books.GetAllAsync(),
            "BookId",
            "Title");
    }

    // GET /Loans/Borrow
    public async Task<IActionResult> Borrow()
    {
        await FillDropdownsAsync();
        return View(new BorrowForm());
    }

    // POST /Loans/Return/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(long id)
    {
        await _loans.ReturnAsync(id);

        return RedirectToAction(nameof(Index));
    }
}