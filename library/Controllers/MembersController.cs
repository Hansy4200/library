using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MembersRepository _member;

    public MembersController(MembersRepository member)
    {
        _member = member;
    }

    public async Task<IActionResult> Index()
    {
        var members = await _member.GetAllAsync();
        return View(members);
    }

    public IActionResult Create()
    {
        return View(new Members());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Members member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        await _member.AddAsync(member);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(long id)
    {
        var member = await _member.GetByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Members member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        await _member.UpdateAsync(member);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long id)
    {
        var member = await _member.GetByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        await _member.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
