using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EFLibrary.Models;
using LibLibrary.Services;

namespace AdminMPA.Pages.Admin
{
    public class EditBookModel : PageModel
    {
        public int BookId { get; set; }
        public int LibCount { get; set; }
        public int Number { get; set; }
        public Book book { get; set; }
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
                return Page();
            }
            return RedirectToPage("../Index");
        }
    }
}
