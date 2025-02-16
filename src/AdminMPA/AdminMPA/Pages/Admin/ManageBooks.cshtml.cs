using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EFLibrary.Models;
using LibLibrary.Services;

namespace AdminMPA.Pages.Admin
{
    public class ManageBooksModel : PageModel
    {
        public List<Book> books { get; set; } = new();
        public void OnGet()
        {
            books.AddRange(LibBooks.GetAllBooks());
        }
    }
}
