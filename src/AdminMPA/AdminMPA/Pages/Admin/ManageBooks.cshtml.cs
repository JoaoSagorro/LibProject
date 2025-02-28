using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EFLibrary.Models;
using LibLibrary.Services;

namespace AdminMPA.Pages.Admin
{
    public class ManageBooksModel : PageModel
    {
        public List<Book> Books { get; set; } = new();
        public int Number { get; set; }
        public IActionResult OnGet()
        {
            Number = 0;
            if (HttpContext.Session.GetString("User") != null)
            {
                Books.AddRange(LibBooks.GetAllBooks());
                return Page();
            }
            return RedirectToPage("../Index");
        }
    }
}
