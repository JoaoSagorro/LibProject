using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EFLibrary.Models;
using LibLibrary.Services;

namespace AdminMPA.Pages.Admin
{
    public class ManageBooksModel : PageModel
    {
        public List<Book> Books { get; set; } = new();
        public void OnGet()
        {
            Books.AddRange(LibBooks.GetAllBooks());
        }
    }
}
