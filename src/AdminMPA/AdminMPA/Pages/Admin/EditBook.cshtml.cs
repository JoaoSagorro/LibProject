using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EFLibrary.Models;
using LibLibrary.Services;

namespace AdminMPA.Pages.Admin
{
    public class EditBookModel : PageModel
    {
        public int BookId { get; set; }
        public Book book { get; set; }

        public IActionResult OnGet(int id)
        {
           if(HttpContext.Session.GetString("User") != null)
            {
            BookId = id; 
            book = LibBooks.GetBookById(id);
                return Page();
            }
            return RedirectToPage("../Index");
        }
    }
}
