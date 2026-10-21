
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class NotesController : Controller
{
    private readonly NoteRepository _notes;

    public NotesController(NoteRepository notes)
    {
        _notes = notes;
    }

    // Display all notes
    public async Task<IActionResult> Index()
    {
        var notes = await _notes.GetAllAsync();
        return View(notes);
    }

    // Display Create form
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Note
        {
            CreatedOn = DateTime.Today
        });
    }

    // Save a new note
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Note note)
    {
        if (!ModelState.IsValid)
            return View(note);

        await _notes.AddAsync(note);

        return RedirectToAction(nameof(Index));
    }

    // Display Edit form
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var note = await _notes.GetByIdAsync(id);

        if (note == null)
            return NotFound();

        return View(note);
    }

    // Update an existing note
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Note note)
    {
        if (id != note.NoteId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(note);

        await _notes.UpdateAsync(note);

        return RedirectToAction(nameof(Index));
    }

    // Display Delete confirmation
    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
        var note = await _notes.GetByIdAsync(id);

        if (note == null)
            return NotFound();

        return View(note);
    }

    // Delete a note
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var note = await _notes.GetByIdAsync(id);

        if (note == null)
            return NotFound();

        await _notes.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}
