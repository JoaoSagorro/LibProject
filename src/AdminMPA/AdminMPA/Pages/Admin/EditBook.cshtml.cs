using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EFLibrary.Models;
using LibLibrary.Services;
using AdminMPA.RazorModels;

namespace AdminMPA.Pages.Admin
{
    public class EditBookModel : PageModel
    {
        [BindProperty]
        public int BookId { get; set; }
        public int LibCount { get; set; }
        public int Number { get; set; }
        
        public Book book { get; set; }
        [BindProperty]
        public EditBook editBook { get; set; }
        public List<Copie> copies { get; set; }

        public IActionResult OnGet(int id)
        {
            Number = 0;
            if(HttpContext.Session.GetString("User") != null)
            {
                BookId = id;
                LibCount = LibCopies.GetLibCount(id);
                book = LibBooks.GetBookById(id);
                copies = LibBooks.GetCopies(book);

                editBook = new EditBook
                {
                    Title = book.Title,
                    Edition = book.Edition,
                    Year = book.Year,
                    AuthorName = book.Author.AuthorName
                };

                return Page();
            }
            return RedirectToPage("../Index");
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                if (!ModelState.IsValid)
                {
                book = LibBooks.GetBookById(BookId);
                copies = LibBooks.GetCopies(book);
                LibCount = LibCopies.GetLibCount(BookId);
                return Page();
                }

            var bookToUpdate = LibBooks.GetBookById(BookId);
            bookToUpdate.Title = editBook.Title;
            bookToUpdate.Edition = editBook.Edition;
            bookToUpdate.Year = editBook.Year;
            if (LibAuthor.AuthorExists(editBook.AuthorName))
            {
                bookToUpdate.Author = LibAuthor.GetAuthorByName(editBook.AuthorName);
            }
            else
            {
                var newAuthor = new Author { AuthorName = editBook.AuthorName };
                LibAuthor.AddAuthor(newAuthor);
                newAuthor = LibAuthor.GetAuthorByName(editBook.AuthorName);
                bookToUpdate.Author = newAuthor;
            }

            LibBooks.EditBook(bookToUpdate);

            return RedirectToPage("./EditBook", new { id = BookId });
            }
            return RedirectToPage("../Index");
        }

        public IActionResult OnPostDelete(int id)
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                    try
                    {
                    var deletedBook = LibBooks.DeleteBookById(id);


                    return RedirectToPage("./ManageBooks");
                    }
                    catch (Exception ex)
                    {
                    ModelState.AddModelError("", $"Error deleting book: {ex.Message}");

                    BookId = id;
                    book = LibBooks.GetBookById(id);
                    copies = LibBooks.GetCopies(book);
                    LibCount = LibCopies.GetLibCount(id);

                    return Page();
                    }
            }
            return RedirectToPage("../Index");
        }
    }
}
