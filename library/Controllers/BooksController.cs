using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class BooksController : Controller
{
    private readonly BookRepository _books;

    public BooksController(BookRepository books)
    {
        _books = books;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _books.GetAllAsync();
        return View(books);
    }

    public IActionResult Create()
    {
        return View(new Book());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        if (!ModelState.IsValid)
        {
            return View(book);
        }

        await _books.AddAsync(book);
        return RedirectToAction(nameof(Index));
    }
}
