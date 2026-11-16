using Npgsql;
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _members;
    private readonly LoanRepository _loans;

    // ASP.NET Core hands in the repositories.
    public MembersController(
        MemberRepository members,
        LoanRepository loans)
    {
        _members = members;
        _loans = loans;
    }

    // Display all members
    public async Task<IActionResult> Index()
    {
        var members = await _members.GetAllAsync();
        return View(members);
    }

    // Display the Create Member form
    public IActionResult Create()
    {
        return View(new Member
        {
            JoinDate = DateOnly.FromDateTime(DateTime.Today)
        });
    }


    // POST /Members/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        try
        {
            await _members.AddAsync(member);
        }
        catch (PostgresException ex) when (
            ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError(
                "Email",
                "Another member already uses this email."
            );

            return View(member);
        }

        return RedirectToAction(nameof(Index));
    }


    // Display the Edit Member form
    public async Task<IActionResult> Edit(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }


    // POST /Members/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        try
        {
            await _members.UpdateAsync(member);
        }
        catch (PostgresException ex) when (
            ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError(
                "Email",
                "Another member already uses this email."
            );

            return View(member);
        }

        return RedirectToAction(nameof(Index));
    }


    // Display the Delete Member confirmation page
    public async Task<IActionResult> Delete(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }


    // POST /Members/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            await _members.DeleteAsync(id);
        }
        catch (PostgresException ex) when (
            ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            var member = await _members.GetByIdAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            ViewBag.Error =
                "This member has loans on record, so the database will not delete them.";

            return View("Delete", member);
        }

        return RedirectToAction(nameof(Index));
    }


    // GET /Members/Details/5
    public async Task<IActionResult> Details(long id)
    {
        // 1. Load the member.
        var member = await _members.GetByIdAsync(id);

        // 2. Check if the member does not exist.
        if (member == null)
        {
            // 3. Return not found.
            return NotFound();
        }

        // 4. Put the member and loans together.
        var details = new MemberDetails
        {
            Member = member,
            Loans = await _loans.GetByMemberAsync(id)
        };

        // 5. Show the page.
        return View(details);
    }
}
